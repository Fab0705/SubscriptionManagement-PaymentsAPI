using System;
using System.Collections.Generic;
using System.Text;
using SubscriptionManagement_PaymentsAPI.Application.Common.Interfaces;
using SubscriptionManagement_PaymentsAPI.Application.Features.Plans.Validators;

namespace SubscriptionManagement_PaymentsAPI.Application.Features.Plans.Commands.UpdatePlan;

public class UpdatePlanCommandValidator : PlanCommandValidatorBase<UpdatePlanCommand>
{
    private readonly IApplicationDbContext _context;
    public UpdatePlanCommandValidator(IApplicationDbContext context)
    {
        _context = context;
        RuleFor(v => v.Name)
            .MustAsync(async (cmd, name, ct) => await BeUniqueName(cmd, name, ct))
                .WithMessage("'{PropertyName}' must be unique.")
                .WithErrorCode("Unique");
    }
    private async Task<bool> BeUniqueName(UpdatePlanCommand command, string name, CancellationToken cancellationToken)
    {
        return !await _context.Plans
            .AnyAsync(l => l.Name == name && l.Id != command.Id, cancellationToken);
    }
}
