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
    public CreatePlanCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<Guid> Handle(CreatePlanCommand request, CancellationToken cancellationToken)
    {
        var entity = new Plan
        {
            Name = request.Name,
            Description = request.Description,
            BillingInterval = request.BillingInterval,
            Price = request.Price,
            StripeProductId = request.StripeProductId,
            StripePriceId = request.StripePriceId,
            IsActive = request.IsActive
        };
        _context.Plans.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }
}
