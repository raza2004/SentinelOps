using SentinelOps.Domain.Enums;

namespace SentinelOps.Application.Common.Models;

public class IncidentDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public IncidentStatus Status { get; set; }
    public Severity Severity { get; set; }
    public Guid? AssignedToId { get; set; }
    public string? AssignedToName { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public DateTime? SlaDeadline { get; set; }
    public bool IsSlaBreach { get; set; }
    public string? AiRootCause { get; set; }
    public string? AiSuggestedFix { get; set; }
    public string? AiSeverityExplanation { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int CommentsCount { get; set; }
    public int AlertsCount { get; set; }
}

public class CreateIncidentDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Severity Severity { get; set; }
    public Guid? AssignedToId { get; set; }
    public int SlaHours { get; set; } = 4;
}

public class UpdateIncidentDto
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public IncidentStatus? Status { get; set; }
    public Severity? Severity { get; set; }
    public Guid? AssignedToId { get; set; }
}

public class IncidentSummaryDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public IncidentStatus Status { get; set; }
    public Severity Severity { get; set; }
    public string? AssignedToName { get; set; }
    public DateTime? SlaDeadline { get; set; }
    public bool IsSlaBreach { get; set; }
    public DateTime CreatedAt { get; set; }
}
