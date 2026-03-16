using System;
using System.Collections.Generic;
using System.Text;

namespace SubscriptionManagement_PaymentsAPI.Application.Features.Customers.Commands.CreateCustomer;

public class CreateCustomerCommandValidator : AbstractValidator<CreateCustomerCommand>
{
    public CreateCustomerCommandValidator()
    {
        RuleFor(v => v.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(255);
    }
}
