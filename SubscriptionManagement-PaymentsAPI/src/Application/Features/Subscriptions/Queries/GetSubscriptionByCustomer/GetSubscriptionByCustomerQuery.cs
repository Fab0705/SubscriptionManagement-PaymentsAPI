using Mapster;
using SubscriptionManagement_PaymentsAPI.Application.Common.Interfaces;
using SubscriptionManagement_PaymentsAPI.Application.Common.Security;
using SubscriptionManagement_PaymentsAPI.Application.Features.Subscriptions.DTOs;

namespace SubscriptionManagement_PaymentsAPI.Application.Features.Subscriptions.Queries.GetSubscriptionByCustomer;

[Authorize]
public record GetSubscriptionByCustomerQuery(Guid customerId) : IRequest<SubscriptionsDTO>;

public class GetSubscriptionByCustomerQueryHandler : IRequestHandler<GetSubscriptionByCustomerQuery, SubscriptionsDTO>
{
    private readonly IApplicationDbContext _context;
    public GetSubscriptionByCustomerQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<SubscriptionsDTO> Handle(GetSubscriptionByCustomerQuery request, CancellationToken cancellationToken)
    {
        var entity = await _context.Subscriptions
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.CustomerId == request.customerId, cancellationToken);
        Guard.Against.NotFound(request.customerId, entity);
        return entity.Adapt<SubscriptionsDTO>();
    }
}
