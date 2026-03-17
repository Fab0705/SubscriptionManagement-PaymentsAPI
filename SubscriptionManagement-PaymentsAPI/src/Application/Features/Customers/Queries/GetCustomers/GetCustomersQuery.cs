using Mapster;
using SubscriptionManagement_PaymentsAPI.Application.Common.Interfaces;
using SubscriptionManagement_PaymentsAPI.Application.Common.Security;
using SubscriptionManagement_PaymentsAPI.Application.Features.Customers.DTO;
using SubscriptionManagement_PaymentsAPI.Domain.Constants;

namespace SubscriptionManagement_PaymentsAPI.Application.Features.Customers.Queries.GetCustomers;

[Authorize(Roles = Roles.Administrator)]
public record GetCustomersQuery : IRequest<List<CustomerDto>>;

public class GetCustomersQueryHandler : IRequestHandler<GetCustomersQuery, List<CustomerDto>>
{
    private readonly IApplicationDbContext _context;
    public GetCustomersQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<List<CustomerDto>> Handle(GetCustomersQuery request, CancellationToken cancellationToken)
    {
        return await _context.Customers
            .AsNoTracking()
            .OrderByDescending(c => c.Created)
            .ProjectToType<CustomerDto>()
            .ToListAsync(cancellationToken);
    }
}
