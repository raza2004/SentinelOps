using Microsoft.EntityFrameworkCore;
using SentinelOps.Domain.Entities;
using SentinelOps.Domain.Enums;
using SentinelOps.Domain.Interfaces;

namespace SentinelOps.Infrastructure.Persistence;

public class AlertRepository : BaseRepository<Alert>, IAlertRepository
{
    public AlertRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<Alert>> GetActiveAlertsAsync() =>
        await _dbSet
            .Where(a => a.Status == AlertStatus.Active)
            .ToListAsync();

    public async Task<IEnumerable<Alert>> GetByIncidentAsync(Guid incidentId) =>
        await _dbSet
            .Where(a => a.IncidentId == incidentId)
            .ToListAsync();
}
