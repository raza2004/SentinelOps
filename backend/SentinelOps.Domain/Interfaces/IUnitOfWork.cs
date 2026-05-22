namespace SentinelOps.Domain.Interfaces;

public interface IUnitOfWork
{
    IIncidentRepository Incidents { get; }
    IAlertRepository Alerts { get; }
    Task<int> SaveChangesAsync();
}
