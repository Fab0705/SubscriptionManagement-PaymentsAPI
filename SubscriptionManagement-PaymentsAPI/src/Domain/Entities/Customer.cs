namespace SubscriptionManagement_PaymentsAPI.Domain.Entities;

public class Customer : BaseAuditableEntity
{
    public string Email { get; set; } = null!;
    public string? StripeCustomerId { get; set; }
}
