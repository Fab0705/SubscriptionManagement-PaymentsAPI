using SubscriptionManagement_PaymentsAPI.Application.Common.Interfaces;

namespace SubscriptionManagement_PaymentsAPI.Application.Features.Plans.Validators;

using FluentValidation;

public abstract class PlanCommandValidatorBase<T> : AbstractValidator<T>
     where T : IPlan
{
    protected PlanCommandValidatorBase()
    {
        RuleFor(v => v.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(v => v.Description)
            .NotEmpty()
            .MaximumLength(1000);

        RuleFor(v => v.BillingInterval)
            .IsInEnum();

        RuleFor(v => v.Price)
            .GreaterThanOrEqualTo(0).WithMessage("'{PropertyName}' must be greater than or equal to 0.")
            .Must(HaveValidScale).WithMessage("'{PropertyName}' cannot have more than 2 decimal places.");
    }

    /*private bool BeAValidInterval(BillingInterval interval)
    {
        var validIntervals = new[] { "weekly", "monthly", "yearly" };
        return validIntervals;
    }*/

    private bool HaveValidScale(decimal price)
    {
        return decimal.Round(price, 2) == price;
    }
}
