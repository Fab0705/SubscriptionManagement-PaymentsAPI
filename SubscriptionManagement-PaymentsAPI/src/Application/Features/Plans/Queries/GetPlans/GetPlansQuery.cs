using Mapster;
using SubscriptionManagement_PaymentsAPI.Application.Common.Interfaces;
using SubscriptionManagement_PaymentsAPI.Application.Features.Plans.DTO;

namespace SubscriptionManagement_PaymentsAPI.Application.Features.Plans.Queries.GetPlans;

public record GetPlansQuery : IRequest<List<PlanDto>>;

public class GetPlansQueryHandler : IRequestHandler<GetPlansQuery, List<PlanDto>>
{
    private readonly IApplicationDbContext _context;
    public GetPlansQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<List<PlanDto>> Handle(GetPlansQuery request, CancellationToken cancellationToken)
    {
        return await _context.Plans
            .AsNoTracking()
            .ProjectToType<PlanDto>()
            .ToListAsync(cancellationToken);
    }
}
