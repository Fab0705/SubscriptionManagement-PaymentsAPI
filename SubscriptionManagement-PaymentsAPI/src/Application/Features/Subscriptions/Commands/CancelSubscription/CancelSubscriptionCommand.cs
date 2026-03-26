using SubscriptionManagement_PaymentsAPI.Application.Common.Interfaces;
using SubscriptionManagement_PaymentsAPI.Application.Common.Security;
using SubscriptionManagement_PaymentsAPI.Domain.Enums;

namespace SubscriptionManagement_PaymentsAPI.Application.Features.Subscriptions.Commands.CancelSubscription;

[Authorize]
public record CancelSubscriptionCommand(Guid SubscriptionId) : IRequest;

public class CancelSubscriptionCommandHandler : IRequestHandler<CancelSubscriptionCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IPaymentGatewayService _paymentGatewayService;
    public CancelSubscriptionCommandHandler(IApplicationDbContext context, IPaymentGatewayService paymentGatewayService)
    {
        _context = context;
        _paymentGatewayService = paymentGatewayService;
    }
    public async Task Handle(CancelSubscriptionCommand request, CancellationToken cancellationToken)
    {
        var subscription = await _context.Subscriptions.FindAsync(new object[] { request.SubscriptionId }, cancellationToken);
        Guard.Against.NotFound(request.SubscriptionId, subscription);

        if (string.IsNullOrEmpty(subscription.StripeSubscriptionId))
        {
            throw new InvalidOperationException("Subscription does not have a valid Stripe subscription ID.");
        }

        await _paymentGatewayService.CancelSubscriptionAsync(subscription.StripeSubscriptionId, cancellationToken);
        
        subscription.CancelAtPeriodEnd = true;

        await _context.SaveChangesAsync(cancellationToken);
    }
}
