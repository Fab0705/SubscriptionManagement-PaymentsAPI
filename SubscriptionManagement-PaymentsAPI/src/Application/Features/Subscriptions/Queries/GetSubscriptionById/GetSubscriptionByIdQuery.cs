
using Mapster;
using SubscriptionManagement_PaymentsAPI.Application.Common.Interfaces;
using SubscriptionManagement_PaymentsAPI.Application.Common.Security;
using SubscriptionManagement_PaymentsAPI.Application.Features.Subscriptions.DTOs;

namespace SubscriptionManagement_PaymentsAPI.Application.Features.Subscriptions.Queries.GetSubscriptionById;

[Authorize]
public record GetSubscriptionByIdQuery(Guid id) : IRequest<SubscriptionsDTO>;

public class GetSubscriptionByIdQueryHandler : IRequestHandler<GetSubscriptionByIdQuery, SubscriptionsDTO>
{
    private readonly IApplicationDbContext _context;
    public GetSubscriptionByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<SubscriptionsDTO> Handle(GetSubscriptionByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _context.Subscriptions
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == request.id, cancellationToken);
        Guard.Against.NotFound(request.id, entity);
        return entity.Adapt<SubscriptionsDTO>();
    }
}
