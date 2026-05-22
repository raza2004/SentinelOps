using SentinelOps.Domain.Interfaces;

namespace SentinelOps.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    public IIncidentRepository Incidents { get; }
    public IAlertRepository Alerts { get; }

    public UnitOfWork(AppDbContext context, IIncidentRepository incidents, IAlertRepository alerts)
    {
        _context = context;
        Incidents = incidents;
        Alerts = alerts;
    }

    public Task<int> SaveChangesAsync() => _context.SaveChangesAsync();
}
