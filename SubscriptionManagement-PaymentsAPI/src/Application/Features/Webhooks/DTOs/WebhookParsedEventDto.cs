namespace SubscriptionManagement_PaymentsAPI.Application.Features.Webhooks.DTOs;

public class WebhookParsedEventDto
{
    public string EventType { get; set; } = string.Empty;
    public string StripeSubscriptionId { get; set; } = string.Empty;
    public Guid? CustomerId { get; set; }
    public Guid? PlanId { get; set; }
    public string? Status { get; set; }
    public DateTime? CurrentPeriodStart { get; set; }
    public DateTime? CurrentPeriodEnd { get; set; }
    public bool CancelAtPeriodEnd { get; set; }
}
