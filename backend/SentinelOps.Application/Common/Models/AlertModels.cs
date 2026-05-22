using SentinelOps.Domain.Enums;

namespace SentinelOps.Application.Common.Models;

public class AlertDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
    public AlertStatus Status { get; set; }
    public Severity Severity { get; set; }
    public double? MetricValue { get; set; }
    public double? Threshold { get; set; }
    public Guid? IncidentId { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateAlertDto
{
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
    public Severity Severity { get; set; }
    public double? MetricValue { get; set; }
    public double? Threshold { get; set; }
}

public class DashboardStatsDto
{
    public int TotalIncidents { get; set; }
    public int OpenIncidents { get; set; }
    public int CriticalIncidents { get; set; }
    public int ResolvedToday { get; set; }
    public int ActiveAlerts { get; set; }
    public int SlaBreaches { get; set; }
    public double AverageResolutionHours { get; set; }
}
