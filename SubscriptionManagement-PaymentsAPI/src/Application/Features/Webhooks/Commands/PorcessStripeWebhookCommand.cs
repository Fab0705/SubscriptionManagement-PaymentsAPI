using Microsoft.Extensions.Configuration;
using SubscriptionManagement_PaymentsAPI.Application.Common.Interfaces;
using SubscriptionManagement_PaymentsAPI.Domain.Entities;
using SubscriptionManagement_PaymentsAPI.Domain.Enums;

namespace SubscriptionManagement_PaymentsAPI.Application.Features.Webhooks.Commands;

public record PorcessStripeWebhookCommand(string JsonPayload, string StripeSignature) : IRequest;

public class PorcessStripeWebhookCommandHandler : IRequestHandler<PorcessStripeWebhookCommand>
{
    private readonly IPaymentGatewayService _paymentGatewayService;
    private readonly IApplicationDbContext _context;
    private readonly IConfiguration _configuration;
    public PorcessStripeWebhookCommandHandler(IPaymentGatewayService paymentGatewayService, IApplicationDbContext context, IConfiguration configuration)
    {
        _paymentGatewayService = paymentGatewayService;
        _context = context;
        _configuration = configuration;
    }
    public async Task Handle(PorcessStripeWebhookCommand request, CancellationToken cancellationToken)
    {
        string webhookSecret = _configuration["Stripe:WebhookSecret"]!;

        var stripeEvent = await _paymentGatewayService.ParseWebhookEventAsync(request.JsonPayload, request.StripeSignature, webhookSecret);

        switch (stripeEvent.EventType)
        {
            case "checkout.session.completed":
                if(stripeEvent.CustomerId.HasValue && stripeEvent.PlanId.HasValue)
                {
                    var customer = await _context.Customers.FindAsync(stripeEvent.CustomerId.Value, cancellationToken);

                    if (customer != null)
                    {
                        customer.StripeCustomerId = stripeEvent.StripeCustomerId;

                        var newSubscription = new Subscription
                        {
                            CustomerId = stripeEvent.CustomerId.Value,
                            PlanId = stripeEvent.PlanId.Value,
                            Status = SubscriptionStatus.Active,
                            StripeSubscriptionId = stripeEvent.StripeSubscriptionId,
                        };
                        _context.Subscriptions.Add(newSubscription);
                    }
                }
                break;
            case "customer.subscription.updated":
            case "customer.subscription.deleted":
                var existingSubscription = await _context.Subscriptions.FirstOrDefaultAsync(s => s.StripeSubscriptionId == stripeEvent.StripeSubscriptionId, cancellationToken);

                if (existingSubscription != null)
                {
                    existingSubscription.CurrentPeriodStart = stripeEvent.CurrentPeriodStart;
                    existingSubscription.CurrentPeriodEnd = stripeEvent.CurrentPeriodEnd;
                    existingSubscription.CancelAtPeriodEnd = stripeEvent.CancelAtPeriodEnd;

                    if (Enum.TryParse<SubscriptionStatus>(stripeEvent.Status, true, out var newStatus))
                    {
                        existingSubscription.Status = newStatus;
                    }
                }
                break;
            case "invoice.paid":
                var suscripcionLocal = await _context.Subscriptions
        .FirstOrDefaultAsync(s => s.StripeSubscriptionId == stripeEvent.StripeSubscriptionId, cancellationToken);

                if (suscripcionLocal != null && stripeEvent.AmountPaid.HasValue)
                {
                    // Parseamos el string que viene de Stripe a tu Enum (ignorando mayúsculas/minúsculas)
                    var statusEnum = Enum.TryParse<InvoiceStatus>(stripeEvent.InvoiceStatus.ToString(), true, out var parsedStatus)
                        ? parsedStatus
                        : InvoiceStatus.Paid; // Por defecto lo ponemos en Paid ya que el evento es invoice.paid

                    var newInvoice = new Invoice
                    {
                        SubscriptionId = suscripcionLocal.Id,
                        StripeInvoiceId = stripeEvent.StripeInvoiceId,
                        Amount = stripeEvent.AmountPaid.Value / 100m,
                        Currency = stripeEvent.Currency ?? "usd",
                        Status = statusEnum.ToString(), // <-- Usamos el Enum aquí
                        PaidAt = DateTime.UtcNow
                    };
                    _context.Invoices.Add(newInvoice);
                    if (stripeEvent.CurrentPeriodStart.HasValue && stripeEvent.CurrentPeriodEnd.HasValue)
                    {
                        suscripcionLocal.CurrentPeriodStart = stripeEvent.CurrentPeriodStart;
                        suscripcionLocal.CurrentPeriodEnd = stripeEvent.CurrentPeriodEnd;
                    }
                }
                break;
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}
