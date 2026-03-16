namespace SubscriptionManagement_PaymentsAPI.Application.Features.Plans.DTO;

public class PlanDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = null!;
    public string Description { get; init; } = null!;
    public string BillingInterval { get; init; } = null!;
    public decimal Price { get; init; }
    public string? StripeProductId { get; init; }
    public string? StripePriceId { get; init; }
    public bool IsActive { get; init; }
}
