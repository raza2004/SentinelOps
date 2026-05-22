using Microsoft.EntityFrameworkCore;
using SentinelOps.Domain.Entities;
using SentinelOps.Domain.Enums;
using SentinelOps.Domain.Interfaces;

namespace SentinelOps.Infrastructure.Persistence;

public class IncidentRepository : BaseRepository<Incident>, IIncidentRepository
{
    public IncidentRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<Incident>> GetActiveIncidentsAsync() =>
        await _dbSet
            .Where(i => i.Status != IncidentStatus.Resolved && i.Status != IncidentStatus.Closed)
            .Include(i => i.AssignedTo)
            .Include(i => i.Alerts)
            .ToListAsync();

    public async Task<IEnumerable<Incident>> GetByAssignedUserAsync(Guid userId) =>
        await _dbSet
            .Where(i => i.AssignedToId == userId)
            .Include(i => i.Alerts)
            .ToListAsync();

    public async Task<IEnumerable<Incident>> GetBreachedSlaIncidentsAsync() =>
        await _dbSet
            .Where(i => i.SlaDeadline < DateTime.UtcNow
                     && i.Status != IncidentStatus.Resolved
                     && i.Status != IncidentStatus.Closed)
            .ToListAsync();
}
