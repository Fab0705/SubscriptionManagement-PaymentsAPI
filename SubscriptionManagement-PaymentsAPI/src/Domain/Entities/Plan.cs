namespace SubscriptionManagement_PaymentsAPI.Domain.Entities;

public class Plan : BaseAuditableEntity
{
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public BillingInterval BillingInterval { get; set; }
    public decimal Price { get; set; }
    public string? StripeProductId { get; set; }
    public string? StripePriceId { get; set; }
    public bool IsActive { get; set; } = true;
}
