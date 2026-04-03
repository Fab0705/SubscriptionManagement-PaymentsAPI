using SubscriptionManagement_PaymentsAPI.Domain.Enums;

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

    public string ? StripeCustomerId { get; set; }


    //For Invoice
    public string? StripeInvoiceId { get; set; }
    public long? AmountPaid { get; set; }
    public string? Currency { get; set; }
    public InvoiceStatus InvoiceStatus { get; set; }
}
