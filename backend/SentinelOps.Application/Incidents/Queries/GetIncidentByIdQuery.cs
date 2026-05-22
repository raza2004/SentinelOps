using MediatR;
using SentinelOps.Application.Common.Models;
using SentinelOps.Domain.Enums;
using SentinelOps.Domain.Interfaces;

namespace SentinelOps.Application.Incidents.Queries;

public record GetIncidentByIdQuery(Guid Id) : IRequest<IncidentDto>;

public class GetIncidentByIdQueryHandler : IRequestHandler<GetIncidentByIdQuery, IncidentDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetIncidentByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IncidentDto> Handle(
        GetIncidentByIdQuery query, CancellationToken cancellationToken)
    {
        var incident = await _unitOfWork.Incidents.GetByIdAsync(query.Id)
            ?? throw new KeyNotFoundException($"Incident {query.Id} not found.");

        var now = DateTime.UtcNow;

        return new IncidentDto
        {
            Id = incident.Id,
            Title = incident.Title,
            Description = incident.Description,
            Status = incident.Status,
            Severity = incident.Severity,
            AssignedToId = incident.AssignedToId,
            AssignedToName = incident.AssignedTo is not null
                ? $"{incident.AssignedTo.FirstName} {incident.AssignedTo.LastName}"
                : null,
            ResolvedAt = incident.ResolvedAt,
            SlaDeadline = incident.SlaDeadline,
            IsSlaBreach = incident.SlaDeadline < now
                       && incident.Status != IncidentStatus.Resolved
                       && incident.Status != IncidentStatus.Closed,
            AiRootCause = incident.AiRootCause,
            AiSuggestedFix = incident.AiSuggestedFix,
            AiSeverityExplanation = incident.AiSeverityExplanation,
            CreatedAt = incident.CreatedAt,
            UpdatedAt = incident.UpdatedAt,
            CommentsCount = incident.IncidentComments?.Count ?? 0,
            AlertsCount = incident.Alerts?.Count ?? 0
        };
    }
}
