using Microsoft.AspNetCore.Identity;
using SubscriptionManagement_PaymentsAPI.Domain.Entities;

namespace SubscriptionManagement_PaymentsAPI.Infrastructure.Identity;

public class ApplicationUser : IdentityUser<Guid>
{
    public Guid? CustomerId { get; set; }
    public Guid? TenantId { get; set; }
    public Tenant? Tenant { get; set; } = null!;
}
