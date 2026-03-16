using SubscriptionManagement_PaymentsAPI.Application.Common.Interfaces;

namespace SubscriptionManagement_PaymentsAPI.Application.Features.Plans.Commands.UpdatePlan;

public record UpdatePlanCommand : IRequest, IPlan
{
    public Guid Id { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required string BillingInterval { get; init; }
    public required decimal Price { get; init; }
    public string? StripeProductId { get; init; }
    public string? StripePriceId { get; init; }
    public bool IsActive { get; init; }
}

public class UpdatePlanCommandHandler : IRequestHandler<UpdatePlanCommand>
{
    private readonly IApplicationDbContext _context;
    public UpdatePlanCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    public async Task Handle(UpdatePlanCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Plans.FindAsync(new object[] { request.Id }, cancellationToken);
        
        Guard.Against.NotFound(request.Id, entity);

        entity.Name = request.Name;
        entity.Description = request.Description;
        entity.BillingInterval = request.BillingInterval;
        entity.Price = request.Price;
        entity.StripeProductId = request.StripeProductId;
        entity.StripePriceId = request.StripePriceId;
        entity.IsActive = request.IsActive;

        await _context.SaveChangesAsync(cancellationToken);
    }
}
