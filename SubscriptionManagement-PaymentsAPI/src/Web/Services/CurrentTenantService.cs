using SubscriptionManagement_PaymentsAPI.Application.Common.Interfaces;

namespace SubscriptionManagement_PaymentsAPI.Web.Services;

public class CurrentTenantService : ICurrentTenantService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentTenantService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid TenantId
    {
        get
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
            {
                return Guid.Empty;
            }

            var tenantClaim = httpContext.User.FindFirst("TenantId");

            if (tenantClaim != null && Guid.TryParse(tenantClaim.Value, out var tenantId))
            {
                return tenantId;
            }

            return Guid.Empty;
        }
    }
}
