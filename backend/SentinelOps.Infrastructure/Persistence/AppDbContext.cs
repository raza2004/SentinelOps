using Microsoft.EntityFrameworkCore;
using SentinelOps.Domain.Entities;

namespace SentinelOps.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Incident> Incidents => Set<Incident>();
    public DbSet<Alert> Alerts => Set<Alert>();
    public DbSet<IncidentComment> IncidentComments => Set<IncidentComment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ── User ────────────────────────────────────────────────────────────
        modelBuilder.Entity<User>(e =>
        {
            e.HasKey(u => u.Id);
            e.Property(u => u.FirstName).HasMaxLength(500);
            e.Property(u => u.LastName).HasMaxLength(500);
            e.Property(u => u.Email).HasMaxLength(256);
            e.Property(u => u.PasswordHash).HasMaxLength(500);
            e.Property(u => u.Role).HasMaxLength(500);
        });

        // ── Incident ─────────────────────────────────────────────────────────
        modelBuilder.Entity<Incident>(e =>
        {
            e.HasKey(i => i.Id);
            e.Property(i => i.Title).HasMaxLength(500);
            e.Property(i => i.Description).HasMaxLength(2000);
            e.Property(i => i.AiRootCause).HasMaxLength(2000);
            e.Property(i => i.AiSuggestedFix).HasMaxLength(2000);
            e.Property(i => i.AiSeverityExplanation).HasMaxLength(2000);

            // Optional relationship to User — no cascade delete
            e.HasOne(i => i.AssignedTo)
             .WithMany(u => u.Incidents)
             .HasForeignKey(i => i.AssignedToId)
             .IsRequired(false)
             .OnDelete(DeleteBehavior.Restrict);
        });

        // ── Alert ─────────────────────────────────────────────────────────────
        modelBuilder.Entity<Alert>(e =>
        {
            e.HasKey(a => a.Id);
            e.Property(a => a.Title).HasMaxLength(500);
            e.Property(a => a.Message).HasMaxLength(2000);
            e.Property(a => a.Source).HasMaxLength(500);

            // Optional relationship to Incident — cascade delete
            e.HasOne(a => a.Incident)
             .WithMany(i => i.Alerts)
             .HasForeignKey(a => a.IncidentId)
             .IsRequired(false)
             .OnDelete(DeleteBehavior.Cascade);
        });

        // ── IncidentComment ───────────────────────────────────────────────────
        modelBuilder.Entity<IncidentComment>(e =>
        {
            e.HasKey(c => c.Id);
            e.Property(c => c.Content).HasMaxLength(2000);

            // Required relationship to Incident — cascade delete
            e.HasOne(c => c.Incident)
             .WithMany(i => i.IncidentComments)
             .HasForeignKey(c => c.IncidentId)
             .IsRequired(true)
             .OnDelete(DeleteBehavior.Cascade);

            // Required relationship to User (Author) — no cascade delete
            e.HasOne(c => c.Author)
             .WithMany()
             .HasForeignKey(c => c.AuthorId)
             .IsRequired(true)
             .OnDelete(DeleteBehavior.Restrict);
        });
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            if (entry.State is EntityState.Added or EntityState.Modified)
                entry.Entity.UpdatedAt = DateTime.UtcNow;
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}
