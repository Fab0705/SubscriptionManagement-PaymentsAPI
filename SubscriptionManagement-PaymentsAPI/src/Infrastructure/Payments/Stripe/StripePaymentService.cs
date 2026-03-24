using SubscriptionManagement_PaymentsAPI.Application.Common.Interfaces;
using SubscriptionManagement_PaymentsAPI.Domain.Enums;
using Stripe;

namespace SubscriptionManagement_PaymentsAPI.Infrastructure.Payments.Stripe;

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
