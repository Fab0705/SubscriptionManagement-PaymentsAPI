using SubscriptionManagement_PaymentsAPI.Application.Common.Interfaces;
using SubscriptionManagement_PaymentsAPI.Application.Common.Security;
using SubscriptionManagement_PaymentsAPI.Domain.Constants;
using SubscriptionManagement_PaymentsAPI.Domain.Entities;
using SubscriptionManagement_PaymentsAPI.Domain.Enums;

namespace SubscriptionManagement_PaymentsAPI.Application.Features.Plans.Commands.CreatePlan;

[Authorize(Roles = Roles.Administrator)]
public record CreatePlanCommand : IRequest<Guid>, IPlan
{
    public required string Name { get; init; }
    public required string Description { get; init; }
    public BillingInterval BillingInterval { get; init; }
    public required decimal Price { get; init; }
    public string? StripeProductId { get; init; }
    public string? StripePriceId { get; init; }
    public bool IsActive { get; init; }
}

public class CreatePlanCommandHandler : IRequestHandler<CreatePlanCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    private readonly IPaymentGatewayService _paymentService;
    public CreatePlanCommandHandler(IApplicationDbContext context, IPaymentGatewayService paymentService)
    {
        _context = context;
        _paymentService = paymentService;
    }
    public async Task<Guid> Handle(CreatePlanCommand request, CancellationToken cancellationToken)
    {
        var (stripeProductId, stripePriceId) = await _paymentService.CreatePlanAsync(
            request.Name,
            request.Description,
            request.Price,
            request.BillingInterval,
            cancellationToken
            );

        var entity = new Plan
        {
            Name = request.Name,
            Description = request.Description,
            BillingInterval = request.BillingInterval,
            Price = request.Price,
            StripeProductId = stripeProductId,
            StripePriceId = stripePriceId,
            IsActive = request.IsActive
        };
        _context.Plans.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }
}
