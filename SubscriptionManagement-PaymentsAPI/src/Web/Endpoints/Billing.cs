using SubscriptionManagement_PaymentsAPI.Application.Features.Billing.Commands.GenerateCustomerPortal;
using SubscriptionManagement_PaymentsAPI.Application.Features.Billing.Queries.GetInvoiceHistory;

namespace SubscriptionManagement_PaymentsAPI.Web.Endpoints;

public class Billing : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapGet(GetInvoiceHistory,"invoices/{subscriptionId}").RequireAuthorization();
        groupBuilder.MapPost(GeneratePortal, "cutomer-portal").RequireAuthorization();
    }
    public async Task<IResult> GetInvoiceHistory(ISender sender, Guid subscriptionId)
    {
        var invoices = await sender.Send(new GetInvoiceHistoryQuery(subscriptionId));
        return Results.Ok(invoices);
    }
    public async Task<string> GeneratePortal(ISender sender, GenerateCustomerPortalCommand command)
    {
        return await sender.Send(command);
    }
}
