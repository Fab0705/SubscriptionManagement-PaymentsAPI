using SubscriptionManagement_PaymentsAPI.Infrastructure.Identity;

namespace SubscriptionManagement_PaymentsAPI.Web.Endpoints;

public class Users : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapIdentityApi<ApplicationUser>();
    }
}
