using Mapster;
using SubscriptionManagement_PaymentsAPI.Application.Common.Interfaces;
using SubscriptionManagement_PaymentsAPI.Application.Features.Plans.DTO;

namespace SubscriptionManagement_PaymentsAPI.Application.Features.Plans.Queries.GetPlanById;

public record GetPlanByIdQuery(Guid id) : IRequest<PlanDto>;

public class GetPlanByIdQueryHandler : IRequestHandler<GetPlanByIdQuery, PlanDto>
{
    private readonly IApplicationDbContext _context;
    public GetPlanByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<PlanDto> Handle(GetPlanByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _context.Plans
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == request.id, cancellationToken);

        Guard.Against.NotFound(request.id, entity);

        return entity.Adapt<PlanDto>();
    }
}
