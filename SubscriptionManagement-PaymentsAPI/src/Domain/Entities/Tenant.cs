namespace SubscriptionManagement_PaymentsAPI.Domain.Entities;

public class Tenant : BaseAuditableEntity
{
    public string Name { get; set; } = null!;
    public string? StripeAccoundId { get; set; }
}
