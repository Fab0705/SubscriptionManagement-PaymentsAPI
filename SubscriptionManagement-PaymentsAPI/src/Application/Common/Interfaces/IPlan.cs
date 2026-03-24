using SubscriptionManagement_PaymentsAPI.Domain.Enums;

namespace SubscriptionManagement_PaymentsAPI.Application.Common.Interfaces;

public interface IPlan
{
    
    string Name { get; }
    string Description { get; }
    BillingInterval BillingInterval { get; }
    decimal Price { get; }
    bool IsActive { get; }
}
