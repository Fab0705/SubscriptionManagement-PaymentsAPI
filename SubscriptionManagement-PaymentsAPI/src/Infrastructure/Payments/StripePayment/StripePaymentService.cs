using SubscriptionManagement_PaymentsAPI.Application.Common.Interfaces;
using SubscriptionManagement_PaymentsAPI.Domain.Enums;
using Stripe;
using Stripe.Checkout;
using SubscriptionManagement_PaymentsAPI.Application.Features.Webhooks.DTOs;

namespace SubscriptionManagement_PaymentsAPI.Infrastructure.Payments.StripePayment;

public class StripePaymentService : IPaymentGatewayService
{
    public async Task ArchivePlanAsync(string stripeProductId, string stripePriceId, CancellationToken cancellationToken)
    {
        var priceService = new PriceService();
        var priceOptions = new PriceUpdateOptions
        {
            Active = false,
        };
        await priceService.UpdateAsync(stripePriceId, priceOptions, cancellationToken: cancellationToken);

        var productService = new ProductService();
        var productOptions = new ProductUpdateOptions
        {
            Active = false,
        };
        await productService.UpdateAsync(stripeProductId, productOptions, cancellationToken: cancellationToken);
    }

    public async Task CancelSubscriptionAsync(string stripeSubscriptionId, CancellationToken cancellationToken)
    {
        var service = new SubscriptionService();

        var options = new SubscriptionUpdateOptions
        {
            CancelAtPeriodEnd = true,
        };

        await service.UpdateAsync(stripeSubscriptionId, options, cancellationToken: cancellationToken);
    }

    public async Task ChangeSubscriptionAsync(string stripeSubscriptionId, string newStripePriceId, CancellationToken cancellationToken)
    {
        var service = new SubscriptionService();

        var subscription = await service.GetAsync(stripeSubscriptionId, cancellationToken: cancellationToken);
        var subscriptionItemId = subscription.Items.Data[0].Id;

        var options = new SubscriptionUpdateOptions
        {
            Items = new List<SubscriptionItemOptions>
            {
                new SubscriptionItemOptions
                {
                    Id = subscriptionItemId,
                    Price = newStripePriceId
                }
            },

            ProrationBehavior = "always_invoice",
        };

        await service.UpdateAsync(stripeSubscriptionId, options, cancellationToken: cancellationToken);
    }

    public async Task<string> CreateCheckoutSessionAsync(string stripePriceId, string customerEmail, Guid customerId, Guid planId, string successUrl, string cancelUrl, CancellationToken cancellationToken)
    {
        var options = new SessionCreateOptions
        {
            CustomerEmail = customerEmail,
            PaymentMethodTypes = new List<string>
            {
                "card",
            },
            LineItems = new List<SessionLineItemOptions>
            {
                new SessionLineItemOptions
                {
                    Price = stripePriceId,
                    Quantity = 1,
                },
            },
            Mode = "subscription",
            SuccessUrl = successUrl,
            CancelUrl = cancelUrl,

            BillingAddressCollection = "auto",

            Metadata = new Dictionary<string, string>
            {
                { "CustomerId", customerId.ToString() },
                { "PlanId", planId.ToString() }
            },

            SubscriptionData = new SessionSubscriptionDataOptions
            {
                Metadata = new Dictionary<string, string>
                {
                    { "CustomerId", customerId.ToString() },
                    { "PlanId", planId.ToString() }
                }
            }
        };

        var service = new SessionService();
        Session session = await service.CreateAsync(options, cancellationToken: cancellationToken);

        return session.Url;
    }

    public async Task<string> CreateCustomerAsync(string email, CancellationToken cancellationToken)
    {
        var options = new CustomerCreateOptions
        {
            Email = email,
        };

        var service = new CustomerService();
        var stripeCustomer = await service.CreateAsync(options, cancellationToken: cancellationToken);

        return stripeCustomer.Id;
    }

    public async Task<(string ProductId, string PriceId)> CreatePlanAsync(string name, string description, decimal price, BillingInterval interval, CancellationToken cancellationToken)
    {
        var productOptions = new ProductCreateOptions
        {
            Name = name,
            Description = description,
            Active = true,
        };

        var productService = new ProductService();
        var product = await productService.CreateAsync(productOptions, cancellationToken: cancellationToken);

        var stripeInterval = interval switch
        {
            BillingInterval.Weekly => "week",
            BillingInterval.Monthly => "month",
            BillingInterval.Yearly => "year",
            _ => throw new ArgumentException(nameof(interval), $"Unsupported billing interval: {interval}")
        };

        var priceOptions = new PriceCreateOptions
        {
            UnitAmount = (long)(price * 100), // Stripe expects amount in cents
            Currency = "usd",
            Recurring = new PriceRecurringOptions
            {
                Interval = stripeInterval,
            },
            Product = product.Id,
        };

        var priceService = new PriceService();
        var stripePrice = await priceService.CreateAsync(priceOptions, cancellationToken: cancellationToken);

        return (product.Id, stripePrice.Id);
    }

    public async Task<string> GenerateCustomerPortalAsync(string stripeCustomerId, string returnUrl, CancellationToken cancellationToken)
    {
        var options = new Stripe.BillingPortal.SessionCreateOptions
        {
            Customer = stripeCustomerId,
            ReturnUrl = returnUrl,
        };

        var service = new Stripe.BillingPortal.SessionService();
        var session = await service.CreateAsync(options, cancellationToken: cancellationToken);

        return session.Url;
    }

    public async Task<WebhookParsedEventDto> ParseWebhookEventAsync(string jsonPayload, string stripeSignature, string webhookSecret)
    {
        var stripeEvent = EventUtility.ConstructEvent(jsonPayload, stripeSignature, webhookSecret);

        var parsedDto = new WebhookParsedEventDto { EventType = stripeEvent.Type };

        if (stripeEvent.Type == EventTypes.CheckoutSessionCompleted)
        {
            var session = stripeEvent.Data.Object as Stripe.Checkout.Session;

            if (session != null)
            {
                parsedDto.StripeSubscriptionId = session.SubscriptionId;

                parsedDto.StripeCustomerId = session.CustomerId;

                if (session.Metadata != null)
                {
                    if (session.Metadata.TryGetValue("CustomerId", out var userIdStr) && Guid.TryParse(userIdStr, out var userId))
                        parsedDto.CustomerId = userId;
                    if (session.Metadata.TryGetValue("PlanId", out var planIdStr) && Guid.TryParse(planIdStr, out var planId))
                        parsedDto.PlanId = planId;
                }
            }
        }
        else if (stripeEvent.Type == EventTypes.CustomerSubscriptionUpdated || stripeEvent.Type == EventTypes.CustomerSubscriptionDeleted)
        {
            var subscription = stripeEvent.Data.Object as Stripe.Subscription;

            if (subscription != null)
            {
                parsedDto.StripeSubscriptionId = subscription.Id;
                parsedDto.Status = subscription.Status;
                parsedDto.CancelAtPeriodEnd = subscription.CancelAtPeriodEnd;
                //parsedDto.CurrentPeriodStart = subscription.CurrentPeriodStart;
                //parsedDto.CurrentPeriodEnd = subscription.CurrentPeriodEnd;
            }
        }
        else if (stripeEvent.Type == EventTypes.InvoicePaid)
        {
            var invoice = stripeEvent.Data.Object as Stripe.Invoice;
            if (invoice != null)
            {
                parsedDto.StripeInvoiceId = invoice.Id;
                parsedDto.AmountPaid = invoice.AmountPaid;
                parsedDto.Currency = invoice.Currency;

                if (Enum.TryParse<InvoiceStatus>(invoice.Status, true, out var parsedStatus))
                {
                    parsedDto.InvoiceStatus = parsedStatus;
                }

                if (invoice.Parent != null && invoice.Parent.SubscriptionDetails != null)
                {
                    parsedDto.StripeSubscriptionId = invoice.Parent.SubscriptionDetails.SubscriptionId;
                }

                if (invoice.Lines != null && invoice.Lines.Data.Count > 0)
                {
                    var firstLineItem = invoice.Lines.Data[0];

                    if (firstLineItem.Period != null)
                    {
                        parsedDto.CurrentPeriodStart = firstLineItem.Period.Start;
                        parsedDto.CurrentPeriodEnd = firstLineItem.Period.End;
                    }
                }
            }
        }

        return await Task.FromResult(parsedDto);
    }

    public async Task<string> UpdatePlanAsync(string stripeProductId, string currentStripePriceId, string name, string description, decimal newPrice, BillingInterval newInterval, CancellationToken cancellationToken)
    {
        var productService = new ProductService();
        var productOptions = new ProductUpdateOptions
        {
            Name = name,
            Description = description
        };
        await productService.UpdateAsync(stripeProductId, productOptions, cancellationToken: cancellationToken);

        var priceService = new PriceService();
        var currentPrice = await priceService.GetAsync(currentStripePriceId, cancellationToken: cancellationToken);

        long newPriceInCents = (long)(newPrice * 100);

        string newStripeInterval = newInterval switch
        {
            BillingInterval.Weekly => "week",
            BillingInterval.Monthly => "month",
            BillingInterval.Yearly => "year",
            _ => throw new ArgumentException(nameof(newStripeInterval), $"Unsupported billing interval: {newInterval}")
        };

        if(currentPrice.UnitAmount != newPriceInCents || currentPrice.Recurring.Interval != newStripeInterval)
        {
            var archivePriceOptions = new PriceUpdateOptions
            {
                Active = false,
            };
            await priceService.UpdateAsync(currentStripePriceId, archivePriceOptions, cancellationToken: cancellationToken);

            var newPriceOptions = new PriceCreateOptions
            {
                UnitAmount = newPriceInCents,
                Currency = "usd",
                Recurring = new PriceRecurringOptions
                {
                    Interval = newStripeInterval,
                    IntervalCount = 1,
                },
                Product = stripeProductId,
            };
            var newlyCreatedPrice = await priceService.CreateAsync(newPriceOptions, cancellationToken: cancellationToken);

            return newlyCreatedPrice.Id;
        }

        return currentStripePriceId;
    }
}
