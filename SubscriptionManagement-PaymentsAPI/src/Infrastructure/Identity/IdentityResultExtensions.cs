using Microsoft.AspNetCore.Identity;
using SubscriptionManagement_PaymentsAPI.Application.Common.Models;

namespace SubscriptionManagement_PaymentsAPI.Infrastructure.Identity;

public static class IdentityResultExtensions
{
    public static Result ToApplicationResult(this IdentityResult result)
    {
        return result.Succeeded
            ? Result.Success()
            : Result.Failure(result.Errors.Select(e => e.Description));
    }
}
