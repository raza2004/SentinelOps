namespace SentinelOps.Application.Common.Interfaces;

public interface INotificationService
{
    Task NotifyIncidentCreated(object incident);
    Task NotifyIncidentUpdated(string incidentId, object incident);
    Task NotifyAlertFired(object alert);
}
