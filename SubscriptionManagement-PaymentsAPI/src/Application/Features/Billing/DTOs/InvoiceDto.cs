namespace SubscriptionManagement_PaymentsAPI.Application.Features.Billing.DTOs;

public class InvoiceDto
{
    public decimal Amount { get; set; }
    public string Currency { get; set; } = null!;
    public string Status { get; set; } = null!;
}
