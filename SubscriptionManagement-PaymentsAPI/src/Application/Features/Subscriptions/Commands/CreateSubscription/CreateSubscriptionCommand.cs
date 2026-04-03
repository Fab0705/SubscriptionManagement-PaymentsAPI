using SubscriptionManagement_PaymentsAPI.Application.Common.Interfaces;
using SubscriptionManagement_PaymentsAPI.Application.Common.Security;

namespace SubscriptionManagement_PaymentsAPI.Application.Features.Subscriptions.Commands.CreateSubscription;

[Authorize]
public record CreateSubscriptionCommand : IRequest<string>
{
    public Guid PlanId { get; set; }
    public required string CustomerEmail { get; set; }
    public required string SuccessUrl { get; set; }
    public required string CancelUrl { get; set; }
}

public class CreateSubscriptionCommandHandler : IRequestHandler<CreateSubscriptionCommand, string>
{
    private readonly IApplicationDbContext _context;
    private readonly IPaymentGatewayService _paymentGatewayService;
    public CreateSubscriptionCommandHandler(IApplicationDbContext context, IPaymentGatewayService paymentGatewayService)
    {
        _context = context;
        _paymentGatewayService = paymentGatewayService;
    }
    public async Task<string> Handle(CreateSubscriptionCommand request, CancellationToken cancellationToken)
    {
        var plan = await _context.Plans.FindAsync(new object[] { request.PlanId }, cancellationToken);
        Guard.Against.NotFound(request.PlanId, plan);

        var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Email == request.CustomerEmail, cancellationToken);
        if (customer == null)
        {
            throw new InvalidOperationException("Customer not found.");
        }

        if (!plan.IsActive || string.IsNullOrEmpty(plan.StripePriceId))
        {
            throw new InvalidOperationException("Selected plan is not available for subscription.");
        }

        string checkoutUrl = await _paymentGatewayService.CreateCheckoutSessionAsync(
            plan.StripePriceId,
            customer.Email,
            customer.Id,
            plan.Id,
            request.SuccessUrl,
            request.CancelUrl,
            cancellationToken);

        return checkoutUrl;
    }
}
