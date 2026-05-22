using MediatR;
using SentinelOps.Application.Common.Models;
using SentinelOps.Domain.Interfaces;

namespace SentinelOps.Application.Alerts.Queries;

public record GetActiveAlertsQuery : IRequest<IEnumerable<AlertDto>>;

public class GetActiveAlertsQueryHandler : IRequestHandler<GetActiveAlertsQuery, IEnumerable<AlertDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetActiveAlertsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<AlertDto>> Handle(
        GetActiveAlertsQuery query, CancellationToken cancellationToken)
    {
        var alerts = await _unitOfWork.Alerts.GetActiveAlertsAsync();

        return alerts.Select(a => new AlertDto
        {
            Id = a.Id,
            Title = a.Title,
            Message = a.Message,
            Source = a.Source,
            Status = a.Status,
            Severity = a.Severity,
            MetricValue = a.MetricValue,
            Threshold = a.Threshold,
            IncidentId = a.IncidentId,
            CreatedAt = a.CreatedAt
        });
    }
}
