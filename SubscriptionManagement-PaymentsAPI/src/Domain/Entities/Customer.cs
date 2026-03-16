namespace SubscriptionManagement_PaymentsAPI.Domain.Entities;

public class Customer : BaseAuditableEntity
{
    public Guid TenantId { get; set; }
    public string Email { get; set; } = null!;
    public string? StripeCustomerId { get; set; }
    
    public Tenant Tenant { get; set; } = null!;
}
