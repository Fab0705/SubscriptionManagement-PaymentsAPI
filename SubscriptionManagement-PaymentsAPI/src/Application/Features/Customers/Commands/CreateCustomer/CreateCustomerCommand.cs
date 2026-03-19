using FluentValidation.Results;
using SubscriptionManagement_PaymentsAPI.Application.Common.Interfaces;
using SubscriptionManagement_PaymentsAPI.Domain.Entities;
using ValidationException = SubscriptionManagement_PaymentsAPI.Application.Common.Exceptions.ValidationException;

namespace SubscriptionManagement_PaymentsAPI.Application.Features.Customers.Commands.CreateCustomer;

public record CreateCustomerCommand : IRequest<Guid>
{
    public string Email { get; init; } = null!;
    public string Password { get; init; } = null!;
    public string? StripeCustomerId { get; init; }
}

public class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    private readonly IIdentityService _identityService;
    public CreateCustomerCommandHandler(IApplicationDbContext context, IIdentityService identityService)
    {
        _context = context;
        _identityService = identityService;
    }
    public async Task<Guid> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        var (result, userId) = await _identityService.CreateUserAsync(request.Email, request.Password);

        if (!result.Succeeded)
        {
            var failures = result.Errors.Select(e => new ValidationFailure("Identity", e));
            throw new ValidationException(failures);
        }

            var entity = new Customer
        {
            Email = request.Email,
                ApplicationUserId = Guid.Parse(userId.ToString()),
                StripeCustomerId = request.StripeCustomerId
        };
        _context.Customers.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }
}
