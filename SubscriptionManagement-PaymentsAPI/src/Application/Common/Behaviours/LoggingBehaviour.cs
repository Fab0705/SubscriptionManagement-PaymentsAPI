using MediatR.Pipeline;
using Microsoft.Extensions.Logging;
using SubscriptionManagement_PaymentsAPI.Application.Common.Interfaces;

namespace SubscriptionManagement_PaymentsAPI.Application.Common.Behaviours;

public class LoggingBehaviour<TRequest> : IRequestPreProcessor<TRequest>
    where TRequest : notnull
{
    private readonly ILogger _logger;
    private readonly IUser _user;
    private readonly IIdentityService _identityService;

    public LoggingBehaviour(ILogger<TRequest> logger, IUser user, IIdentityService identityService)
    {
        _logger = logger;
        _user = user;
        _identityService = identityService;
    }

    public async Task Process(TRequest request, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var userId = _user.Id ?? string.Empty;
        string? userName = string.Empty;

        if (!string.IsNullOrEmpty(userId) && Guid.TryParse(userId, out var parsedUserId))
        {
            userName = await _identityService.GetUserNameAsync(parsedUserId);
        }

        _logger.LogInformation("SubscriptionManagement_PaymentsAPI Request: {Name} {@UserId} {@UserName} {@Request}",
            requestName, userId, userName, request);
    }
}
