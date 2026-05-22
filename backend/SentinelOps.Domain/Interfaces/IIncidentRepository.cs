using SentinelOps.Domain.Entities;

namespace SentinelOps.Domain.Interfaces;

public interface IIncidentRepository : IRepository<Incident>
{
    Task<IEnumerable<Incident>> GetActiveIncidentsAsync();
    Task<IEnumerable<Incident>> GetByAssignedUserAsync(Guid userId);
    Task<IEnumerable<Incident>> GetBreachedSlaIncidentsAsync();
}
