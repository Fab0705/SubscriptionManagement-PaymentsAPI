using System;
using System.Collections.Generic;
using System.Text;
using Mapster;
using SubscriptionManagement_PaymentsAPI.Application.Common.Interfaces;
using SubscriptionManagement_PaymentsAPI.Application.Common.Security;
using SubscriptionManagement_PaymentsAPI.Application.Features.Subscriptions.DTOs;
using SubscriptionManagement_PaymentsAPI.Domain.Constants;

namespace SubscriptionManagement_PaymentsAPI.Application.Features.Subscriptions.Queries.GetSubscriptions;

[Authorize(Roles = Roles.Administrator)]
public record GetSubscriptionsQuery : IRequest<List<SubscriptionsDTO>>;

public class GetSubscriptionsQueryHandler : IRequestHandler<GetSubscriptionsQuery, List<SubscriptionsDTO>>
{
    private readonly IApplicationDbContext _context;
    public GetSubscriptionsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<List<SubscriptionsDTO>> Handle(GetSubscriptionsQuery request, CancellationToken cancellationToken)
    {
        var subscriptions = await _context.Subscriptions
            .AsNoTracking()
            .OrderByDescending(s => s.CurrentPeriodStart)
            .ProjectToType<SubscriptionsDTO>()
            .ToListAsync(cancellationToken);
        return subscriptions;
    }
}
