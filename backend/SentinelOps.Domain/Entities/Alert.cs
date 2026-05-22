using SentinelOps.Domain.Enums;

namespace SentinelOps.Domain.Entities;

public class Alert : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
    public AlertStatus Status { get; set; }
    public Severity Severity { get; set; }
    public double? MetricValue { get; set; }
    public double? Threshold { get; set; }
    public Guid? IncidentId { get; set; }

    public Incident? Incident { get; set; }
}
