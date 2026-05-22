using Microsoft.AspNetCore.SignalR;
using SentinelOps.Application.Common.Interfaces;
using SentinelOps.Infrastructure.Hubs;

namespace SentinelOps.Infrastructure.Services;

public class NotificationService : INotificationService
{
    private readonly IHubContext<IncidentHub, IIncidentHubClient> _hubContext;

    public NotificationService(IHubContext<IncidentHub, IIncidentHubClient> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task NotifyIncidentCreated(object incident) =>
        await _hubContext.Clients.All.IncidentCreated(incident);

    public async Task NotifyIncidentUpdated(string incidentId, object incident) =>
        await _hubContext.Clients.Group(incidentId).IncidentUpdated(incident);

    public async Task NotifyAlertFired(object alert) =>
        await _hubContext.Clients.All.AlertFired(alert);
}
