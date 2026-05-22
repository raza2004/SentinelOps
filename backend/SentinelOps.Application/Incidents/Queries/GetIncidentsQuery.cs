using MediatR;
using SentinelOps.Application.Common.Models;
using SentinelOps.Domain.Enums;
using SentinelOps.Domain.Interfaces;

namespace SentinelOps.Application.Incidents.Queries;

public record GetIncidentsQuery(
    IncidentStatus? Status,
    Severity? Severity
) : IRequest<IEnumerable<IncidentSummaryDto>>;

public class GetIncidentsQueryHandler : IRequestHandler<GetIncidentsQuery, IEnumerable<IncidentSummaryDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetIncidentsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<IncidentSummaryDto>> Handle(
        GetIncidentsQuery query, CancellationToken cancellationToken)
    {
        var incidents = await _unitOfWork.Incidents.GetAllAsync();

        if (query.Status.HasValue)
            incidents = incidents.Where(i => i.Status == query.Status.Value);

        if (query.Severity.HasValue)
            incidents = incidents.Where(i => i.Severity == query.Severity.Value);

        var now = DateTime.UtcNow;

        return incidents.Select(i => new IncidentSummaryDto
        {
            Id = i.Id,
            Title = i.Title,
            Status = i.Status,
            Severity = i.Severity,
            AssignedToName = i.AssignedTo is not null
                ? $"{i.AssignedTo.FirstName} {i.AssignedTo.LastName}"
                : null,
            SlaDeadline = i.SlaDeadline,
            IsSlaBreach = i.SlaDeadline < now
                       && i.Status != IncidentStatus.Resolved
                       && i.Status != IncidentStatus.Closed,
            CreatedAt = i.CreatedAt
        });
    }
}
