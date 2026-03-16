namespace SubscriptionManagement_PaymentsAPI.Domain.Entities;

public class Subscription : BaseAuditableEntity
{
    public Guid TenantId { get; set; }
    public Guid CustomerId { get; set; }
    public Guid PlanId { get; set; }
    public string? StripeSubscriptionId { get; set; }
    public string Status { get; set; } = null!;
    public DateTime? CurrentPeriodStart { get; set; }
    public DateTime? CurrentPeriodEnd { get; set; }
    public bool CancelAtPeriodEnd { get; set; } = false;

    public Tenant Tenant { get; set; } = null!;
    public Customer Customer { get; set; } = null!;
    public Plan Plan { get; set; } = null!;
}
