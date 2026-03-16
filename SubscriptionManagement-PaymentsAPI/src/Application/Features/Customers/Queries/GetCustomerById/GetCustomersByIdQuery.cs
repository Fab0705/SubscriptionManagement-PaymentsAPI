using Mapster;
using SubscriptionManagement_PaymentsAPI.Application.Common.Interfaces;
using SubscriptionManagement_PaymentsAPI.Application.Common.Security;
using SubscriptionManagement_PaymentsAPI.Application.Features.Customers.DTO;

namespace SubscriptionManagement_PaymentsAPI.Application.Features.Customers.Queries.GetCustomerById;

[Authorize]
public record GetCustomersByIdQuery(Guid id) : IRequest<CustomerDto>;

public class GetCustomersByIdQueryHandler : IRequestHandler<GetCustomersByIdQuery, CustomerDto>
{
    private readonly IApplicationDbContext _context;
    public GetCustomersByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<CustomerDto> Handle(GetCustomersByIdQuery request, CancellationToken cancellationToken)
    {
        var customer = await _context.Customers
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == request.id, cancellationToken);

        Guard.Against.NotFound(request.id, customer);

        return customer.Adapt<CustomerDto>();
    }
}
