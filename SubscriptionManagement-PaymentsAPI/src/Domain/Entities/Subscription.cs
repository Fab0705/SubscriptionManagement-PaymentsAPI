namespace SubscriptionManagement_PaymentsAPI.Domain.Entities;

public class Subscription : BaseAuditableEntity
{
    public Guid CustomerId { get; set; }
    public Guid PlanId { get; set; }
    public string? StripeSubscriptionId { get; set; }
    public SubscriptionStatus Status { get; set; } = SubscriptionStatus.Incomplete;
    public DateTime? CurrentPeriodStart { get; set; }
    public DateTime? CurrentPeriodEnd { get; set; }
    public bool CancelAtPeriodEnd { get; set; } = false;

    public Customer Customer { get; set; } = null!;
    public Plan Plan { get; set; } = null!;
}
