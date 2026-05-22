using SentinelOps.Domain.Enums;

namespace SentinelOps.Domain.Entities;

public class Incident : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public IncidentStatus Status { get; set; }
    public Severity Severity { get; set; }
    public Guid? AssignedToId { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public DateTime? SlaDeadline { get; set; }
    public string? AiRootCause { get; set; }
    public string? AiSuggestedFix { get; set; }
    public string? AiSeverityExplanation { get; set; }

    public User? AssignedTo { get; set; }
    public List<Alert> Alerts { get; set; } = [];
    public List<IncidentComment> IncidentComments { get; set; } = [];
}
