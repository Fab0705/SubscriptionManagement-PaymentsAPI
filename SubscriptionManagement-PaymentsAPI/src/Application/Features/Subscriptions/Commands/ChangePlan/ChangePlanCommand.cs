using SubscriptionManagement_PaymentsAPI.Application.Common.Interfaces;
using SubscriptionManagement_PaymentsAPI.Application.Common.Security;
using SubscriptionManagement_PaymentsAPI.Domain.Enums;

namespace SubscriptionManagement_PaymentsAPI.Application.Features.Subscriptions.Commands.ChangePlan;

[Authorize]
public record ChangePlanCommand : IRequest
{
    public Guid SubscriptionId { get; init; }
    public Guid NewPlanId { get; init; }
}

public class ChangePlanCommandHandler : IRequestHandler<ChangePlanCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IPaymentGatewayService _paymentGatewayService;
    public ChangePlanCommandHandler(IApplicationDbContext context, IPaymentGatewayService paymentGatewayService)
    {
        _context = context;
        _paymentGatewayService = paymentGatewayService;
    }
    public async Task Handle(ChangePlanCommand request, CancellationToken cancellationToken)
    {
        var subscription = await _context.Subscriptions.FindAsync(new object[] { request.SubscriptionId }, cancellationToken);
        Guard.Against.NotFound(request.SubscriptionId, subscription);
        var newPlan = await _context.Plans.FindAsync(new object[] { request.NewPlanId }, cancellationToken);
        Guard.Against.NotFound(request.NewPlanId, newPlan);
        if (string.IsNullOrEmpty(subscription.StripeSubscriptionId))
        {
            throw new InvalidOperationException("Subscription does not have a valid Stripe subscription ID.");
        }
        if (!newPlan.IsActive || string.IsNullOrEmpty(newPlan.StripePriceId))
        {
            throw new InvalidOperationException("Selected plan is not available for subscription.");
        }
        await _paymentGatewayService.ChangeSubscriptionAsync(subscription.StripeSubscriptionId, newPlan.StripePriceId, cancellationToken);
        
        subscription.PlanId = newPlan.Id;
        subscription.Status = SubscriptionStatus.Active;
        await _context.SaveChangesAsync(cancellationToken);
    }
}
