namespace SentinelOps.Domain.Entities;

public class IncidentComment : BaseEntity
{
    public string Content { get; set; } = string.Empty;
    public Guid IncidentId { get; set; }
    public Guid AuthorId { get; set; }

    public Incident Incident { get; set; } = null!;
    public User Author { get; set; } = null!;
}
