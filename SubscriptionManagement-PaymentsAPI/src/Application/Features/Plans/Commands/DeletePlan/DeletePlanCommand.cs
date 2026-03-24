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
    private readonly IPaymentGatewayService _paymentService;
    public DeletePlanCommandHandler(IApplicationDbContext context, IPaymentGatewayService paymentService)
    {
        _context = context;
        _paymentService = paymentService;
    }
    public async Task Handle(DeletePlanCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Plans.FindAsync(new object[] { request.id }, cancellationToken);
        Guard.Against.NotFound(request.id, entity);

        if (!string.IsNullOrEmpty(entity.StripeProductId) && !string.IsNullOrEmpty(entity.StripePriceId))
        {
            await _paymentService.ArchivePlanAsync(entity.StripeProductId, entity.StripePriceId, cancellationToken);
        }

        entity.IsActive = false;

        await _context.SaveChangesAsync(cancellationToken);
    }
}
