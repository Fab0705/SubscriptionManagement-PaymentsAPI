using SubscriptionManagement_PaymentsAPI.Application.Common.Interfaces;
using SubscriptionManagement_PaymentsAPI.Domain.Entities;

namespace SubscriptionManagement_PaymentsAPI.Application.Features.Customers.Commands.CreateCustomer;

public record CreateCustomerCommand : IRequest<Guid>
{
    public string Email { get; init; } = null!;
    public string? StripeCustomerId { get; init; }
}

public class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    public CreateCustomerCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<Guid> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        var entity = new Customer
        {
            Email = request.Email,
            StripeCustomerId = request.StripeCustomerId
        };
        _context.Customers.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }
}
