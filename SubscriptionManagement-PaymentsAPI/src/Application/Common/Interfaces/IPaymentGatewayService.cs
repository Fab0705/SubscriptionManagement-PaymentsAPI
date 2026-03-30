using SubscriptionManagement_PaymentsAPI.Application.Features.Webhooks.DTOs;
using SubscriptionManagement_PaymentsAPI.Domain.Enums;

namespace SubscriptionManagement_PaymentsAPI.Application.Common.Interfaces;
public interface IPaymentGatewayService
{
    Task<(string ProductId, string PriceId)> CreatePlanAsync(string name, string description, decimal price, BillingInterval interval, CancellationToken cancellationToken);
    Task<string> UpdatePlanAsync(string stripeProductId, string currentStripePriceId, string name, string description, decimal newPrice, BillingInterval newInterval, CancellationToken cancellationToken);
    Task ArchivePlanAsync(string stripeProductId, string stripePriceId, CancellationToken cancellationToken);
    Task<string> CreateCheckoutSessionAsync(string stripePriceId, string customerEmail, string successUrl, string cancelUrl, CancellationToken cancellationToken);
    Task CancelSubscriptionAsync(string stripeSubscriptionId, CancellationToken cancellationToken);
    Task ChangeSubscriptionAsync(string stripeSubscriptionId, string newStripePriceId, CancellationToken cancellationToken);
    Task<WebhookParsedEventDto> ParseWebhookEventAsync(string jsonPayload, string stripeSignature, string webhookSecret);
    Task<string> GenerateCustomerPortalAsync(string stripeCustomerId, string returnUrl, CancellationToken cancellationToken);
}
