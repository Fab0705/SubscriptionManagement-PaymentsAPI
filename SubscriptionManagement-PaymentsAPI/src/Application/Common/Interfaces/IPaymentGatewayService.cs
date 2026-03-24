using SubscriptionManagement_PaymentsAPI.Domain.Enums;

namespace SubscriptionManagement_PaymentsAPI.Application.Common.Interfaces;
public interface IPaymentGatewayService
{
    Task<(string ProductId, string PriceId)> CreatePlanAsync(string name, string description, decimal price, BillingInterval interval, CancellationToken cancellationToken);
    Task<string> UpdatePlanAsync(string stripeProductId, string currentStripePriceId, string name, string description, decimal newPrice, BillingInterval newInterval, CancellationToken cancellationToken);
    Task ArchivePlanAsync(string stripeProductId, string stripePriceId, CancellationToken cancellationToken);
}
