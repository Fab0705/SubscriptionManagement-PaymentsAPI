using SubscriptionManagement_PaymentsAPI.Application.Common.Interfaces;
using SubscriptionManagement_PaymentsAPI.Application.Common.Security;
using SubscriptionManagement_PaymentsAPI.Domain.Constants;

namespace SubscriptionManagement_PaymentsAPI.Application.Features.Plans.Commands.DeletePlan;

[Authorize(Roles = Roles.Administrator)]
[Authorize(Policy = Policies.CanDelete)]
public record DeletePlanCommand(Guid id) : IRequest;

public class DeletePlanCommandHandler : IRequestHandler<DeletePlanCommand>
{
    private readonly IApplicationDbContext _context;
    public DeletePlanCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    public async Task Handle(DeletePlanCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Plans.FindAsync(new object[] { request.id }, cancellationToken);
        Guard.Against.NotFound(request.id, entity);
        _context.Plans.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
