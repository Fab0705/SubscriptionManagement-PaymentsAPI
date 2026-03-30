using SubscriptionManagement_PaymentsAPI.Application.Common.Interfaces;
using SubscriptionManagement_PaymentsAPI.Application.Common.Security;

namespace SubscriptionManagement_PaymentsAPI.Application.Features.Billing.Commands.GenerateCustomerPortal;

[Authorize]
public record GenerateCustomerPortalCommand : IRequest<string>
{
    public required string StripeCustomerId { get; init; }
    public required string ReturnUrl { get; init; }
}

public class GenerateCustomerPortalCommandHandler : IRequestHandler<GenerateCustomerPortalCommand, string>
{
    private readonly IPaymentGatewayService _paymentGatewayService;
    public GenerateCustomerPortalCommandHandler(IPaymentGatewayService paymentGatewayService)
    {
        _paymentGatewayService = paymentGatewayService;
    }
    public async Task<string> Handle(GenerateCustomerPortalCommand request, CancellationToken cancellationToken)
    {
        return await _paymentGatewayService.GenerateCustomerPortalAsync(request.StripeCustomerId, request.ReturnUrl, cancellationToken);
    }
}
