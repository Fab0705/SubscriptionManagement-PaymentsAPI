using Microsoft.AspNetCore.Http.HttpResults;
using SubscriptionManagement_PaymentsAPI.Application.Common.Models;
using SubscriptionManagement_PaymentsAPI.Application.Features.Customers.Commands.CreateCustomer;
using SubscriptionManagement_PaymentsAPI.Application.Features.Customers.DTO;
using SubscriptionManagement_PaymentsAPI.Application.Features.Customers.Queries.GetCustomerById;
using SubscriptionManagement_PaymentsAPI.Application.Features.Customers.Queries.GetCustomers;

namespace SubscriptionManagement_PaymentsAPI.Web.Endpoints;

public class Customer : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapGet(GetCustomers).RequireAuthorization();
        groupBuilder.MapGet(GetCustomerById, "{id}").RequireAuthorization();
        groupBuilder.MapPost(CreateCustomer).RequireAuthorization();
    }

    public async Task<Ok<List<CustomerDto>>> GetCustomers(ISender sender)
    {
        var vm = await sender.Send(new GetCustomersQuery());
        return TypedResults.Ok(vm);
    }

    public async Task<Ok<CustomerDto>> GetCustomerById(ISender sender, [AsParameters] GetCustomersByIdQuery query)
    {
        var result = await sender.Send(query);
        return TypedResults.Ok(result);
    }

    public async Task<Created<Guid>> CreateCustomer(ISender sender, CreateCustomerCommand command)
    {
        var id = await sender.Send(command);
        return TypedResults.Created($"/{nameof(Customer)}/{id}", id);
    }
}
