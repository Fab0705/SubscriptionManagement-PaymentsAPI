using Mapster;
using SubscriptionManagement_PaymentsAPI.Application.Common.Interfaces;
using SubscriptionManagement_PaymentsAPI.Application.Features.Billing.DTOs;

namespace SubscriptionManagement_PaymentsAPI.Application.Features.Billing.Queries.GetInvoiceHistory;

public record GetInvoiceHistoryQuery(Guid id) : IRequest<List<InvoiceDto>>;

public class GetInvoiceHistoryQueryHandler : IRequestHandler<GetInvoiceHistoryQuery, List<InvoiceDto>>
{
    private readonly IApplicationDbContext _context;
    public GetInvoiceHistoryQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<List<InvoiceDto>> Handle(GetInvoiceHistoryQuery request, CancellationToken cancellationToken)
    {
        var invoices = await _context.Invoices
            .AsNoTracking()
            .Where(i => i.SubscriptionId == request.id)
            .OrderByDescending(i => i.PaidAt)
            .ToListAsync(cancellationToken);
        return invoices.Adapt<List<InvoiceDto>>();
    }
}
