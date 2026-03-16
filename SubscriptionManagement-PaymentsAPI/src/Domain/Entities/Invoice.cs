namespace SubscriptionManagement_PaymentsAPI.Domain.Entities;

public class Invoice : BaseAuditableEntity
{
    public Guid TenantId { get; set; }
    public Guid SubscriptionId { get; set; }
    public string? StripeInvoiceId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = null!;
    public string Status { get; set; } = null!;
    public DateTime? PaidAt { get; set; }
    public Tenant Tenant { get; set; } = null!;
    public Subscription Subscription { get; set; } = null!;
}
