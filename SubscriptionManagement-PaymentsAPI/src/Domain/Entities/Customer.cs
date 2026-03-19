namespace SubscriptionManagement_PaymentsAPI.Domain.Entities;

public class Customer : BaseAuditableEntity
{
    public Guid ApplicationUserId { get; set; }
    public string Email { get; set; } = null!;
    public string? StripeCustomerId { get; set; }
}
