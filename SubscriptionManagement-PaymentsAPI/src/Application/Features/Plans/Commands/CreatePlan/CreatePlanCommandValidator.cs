using SubscriptionManagement_PaymentsAPI.Application.Common.Interfaces;
using SubscriptionManagement_PaymentsAPI.Application.Features.Plans.Validators;

namespace SubscriptionManagement_PaymentsAPI.Application.Features.Plans.Commands.CreatePlan;

public class CreatePlanCommandValidator : PlanCommandValidatorBase<CreatePlanCommand>
{
    private readonly IApplicationDbContext _context;

    public CreatePlanCommandValidator(IApplicationDbContext context)
    {
        _context = context;

        RuleFor(v => v.Name)
            .MustAsync(BeUniqueName)
                .WithMessage("'{PropertyName}' must be unique.")
                .WithErrorCode("Unique");
    }

    private async Task<bool> BeUniqueName(string name, CancellationToken cancellationToken)
    {
        return !await _context.Plans
            .AnyAsync(l => l.Name == name, cancellationToken);
    }
}
