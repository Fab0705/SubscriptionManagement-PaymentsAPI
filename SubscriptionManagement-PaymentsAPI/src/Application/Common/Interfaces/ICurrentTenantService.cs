namespace SubscriptionManagement_PaymentsAPI.Application.Common.Interfaces;

public interface ICurrentTenantService
{
    Guid TenantId { get; }
}
