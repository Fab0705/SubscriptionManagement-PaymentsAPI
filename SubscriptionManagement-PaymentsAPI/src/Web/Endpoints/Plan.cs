using Microsoft.AspNetCore.Http.HttpResults;
using SubscriptionManagement_PaymentsAPI.Application.Features.Plans.Commands.CreatePlan;
using SubscriptionManagement_PaymentsAPI.Application.Features.Plans.Commands.DeletePlan;
using SubscriptionManagement_PaymentsAPI.Application.Features.Plans.Commands.UpdatePlan;
using SubscriptionManagement_PaymentsAPI.Application.Features.Plans.DTO;
using SubscriptionManagement_PaymentsAPI.Application.Features.Plans.Queries.GetPlanById;
using SubscriptionManagement_PaymentsAPI.Application.Features.Plans.Queries.GetPlans;

namespace SubscriptionManagement_PaymentsAPI.Web.Endpoints;

public class Plan : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapGet(GetPlans).RequireAuthorization();
        groupBuilder.MapGet(GetPlanById, "{id}").RequireAuthorization();
        groupBuilder.MapPost(CreatePlan).RequireAuthorization();
        groupBuilder.MapPut(UpdatePlan, "{id}").RequireAuthorization();
        groupBuilder.MapDelete(DeletePlan, "{id}").RequireAuthorization();
    }

    public async Task<Ok<List<PlanDto>>> GetPlans(ISender sender)
    {
        var vm = await sender.Send(new GetPlansQuery());
        return TypedResults.Ok(vm);
    }

    public async Task<Ok<PlanDto>> GetPlanById(ISender sender, [AsParameters] GetPlanByIdQuery query)
    {
        var result = await sender.Send(query);
        return TypedResults.Ok(result);
    }

    public async Task<Created<Guid>> CreatePlan(ISender sender, CreatePlanCommand command)
    {
        var id = await sender.Send(command);
        return TypedResults.Created($"/{nameof(Plan)}/{id}", id);
    }
    public async Task<Results<NoContent, BadRequest>> UpdatePlan(ISender sender, Guid id, UpdatePlanCommand command)
    {
        if (id != command.Id) return TypedResults.BadRequest();
        await sender.Send(command);
        return TypedResults.NoContent();
    }
    public async Task<NoContent> DeletePlan(ISender sender, Guid id)
    {
        await sender.Send(new DeletePlanCommand(id));
        return TypedResults.NoContent();
    }
}
