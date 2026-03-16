namespace SubscriptionManagement_PaymentsAPI.Application.Common.Interfaces;

public interface IPlan
{
    
    string Name { get; }
    string Description { get; }
    string BillingInterval { get; }
    decimal Price { get; }
    bool IsActive { get; }
}
