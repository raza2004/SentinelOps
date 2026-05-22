using Microsoft.AspNetCore.SignalR;

namespace SentinelOps.Infrastructure.Hubs;

public interface IIncidentHubClient
{
    Task IncidentCreated(object incident);
    Task IncidentUpdated(object incident);
    Task AlertFired(object alert);
    Task AiAnalysisComplete(object analysis);
}

public class IncidentHub : Hub<IIncidentHubClient>
{
    public async Task JoinIncidentRoom(string incidentId) =>
        await Groups.AddToGroupAsync(Context.ConnectionId, incidentId);

    public async Task LeaveIncidentRoom(string incidentId) =>
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, incidentId);
}
