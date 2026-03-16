namespace SubscriptionManagement_PaymentsAPI.Domain.Entities;

public class Plan : BaseAuditableEntity
{
    public Guid TenantId { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string BillingInterval { get; set; } = null!;
    public decimal Price { get; set; }
    public string? StripeProductId { get; set; }
    public string? StripePriceId { get; set; }
    public bool IsActive { get; set; } = true;

    public Tenant Tenant { get; set; } = null!;
}
