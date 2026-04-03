using SubscriptionManagement_PaymentsAPI.Application.Features.Subscriptions.Commands.CancelSubscription;
using SubscriptionManagement_PaymentsAPI.Application.Features.Subscriptions.Commands.ChangePlan;
using SubscriptionManagement_PaymentsAPI.Application.Features.Subscriptions.Commands.CreateSubscription;
using SubscriptionManagement_PaymentsAPI.Application.Features.Subscriptions.DTOs;
using SubscriptionManagement_PaymentsAPI.Application.Features.Subscriptions.Queries.GetSubscriptionByCustomer;
using SubscriptionManagement_PaymentsAPI.Application.Features.Subscriptions.Queries.GetSubscriptionById;
using SubscriptionManagement_PaymentsAPI.Application.Features.Subscriptions.Queries.GetSubscriptions;

namespace SubscriptionManagement_PaymentsAPI.Web.Endpoints;

public class Subcriptions : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapGet(GetSubscriptions).RequireAuthorization();
        groupBuilder.MapGet(GetSubscriptionById, "{id}").RequireAuthorization();
        groupBuilder.MapGet(GetSubscriptionByCustomerId, "{customerId}/customer").RequireAuthorization();
        groupBuilder.MapPost(CreateSubscription).RequireAuthorization();
        groupBuilder.MapPut(UpdateSubscription, "{id}/change-plan").RequireAuthorization();
        groupBuilder.MapPut("/cancel", CancelSubscription).RequireAuthorization();
    }
    public async Task<List<SubscriptionsDTO>> GetSubscriptions(ISender sender)
    {
        return await sender.Send(new GetSubscriptionsQuery());
    }
    public async Task<SubscriptionsDTO> GetSubscriptionById(ISender sender, [AsParameters] GetSubscriptionByIdQuery query)
    {
        return await sender.Send(query);
    }
    public async Task<SubscriptionsDTO> GetSubscriptionByCustomerId(ISender sender, [AsParameters] GetSubscriptionByCustomerQuery query)
    {
        return await sender.Send(query);
    }
    public async Task<string> CreateSubscription(ISender sender, CreateSubscriptionCommand command)
    {
        return await sender.Send(command);
    }
    public async Task<IResult> UpdateSubscription(ISender sender, Guid id, ChangePlanCommand command)
    {
        if (id != command.SubscriptionId) return Results.BadRequest("Subscription ID in the URL does not match the ID in the request body.");
        await sender.Send(command);
        return Results.NoContent();
    }
    public async Task<IResult> CancelSubscription(ISender sender, Guid id)
    {
        await sender.Send(new CancelSubscriptionCommand(id));
        return Results.NoContent();
    }
}
