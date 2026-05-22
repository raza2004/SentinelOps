using SentinelOps.Domain.Entities;

namespace SentinelOps.Domain.Interfaces;

public interface IAlertRepository : IRepository<Alert>
{
    Task<IEnumerable<Alert>> GetActiveAlertsAsync();
    Task<IEnumerable<Alert>> GetByIncidentAsync(Guid incidentId);
}
