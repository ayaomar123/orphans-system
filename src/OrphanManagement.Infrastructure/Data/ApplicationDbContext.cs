using Microsoft.EntityFrameworkCore;
using OrphanManagement.Domain.Entities;
using OrphanManagement.Infrastructure.Data.Configurations;

namespace OrphanManagement.Infrastructure.Data;

/// <summary>
/// Main database context for the Orphan Management System.
/// Configures entities and their relationships using Fluent API.
/// </summary>
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    // DbSets for all entities
    public DbSet<User> Users => Set<User>();
    public DbSet<Orphan> Orphans => Set<Orphan>();
    public DbSet<Event> Events => Set<Event>();
    public DbSet<OrphanEvent> OrphanEvents => Set<OrphanEvent>();
    public DbSet<Sponsorship> Sponsorships => Set<Sponsorship>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply entity configurations
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new OrphanConfiguration());
        modelBuilder.ApplyConfiguration(new EventConfiguration());
        modelBuilder.ApplyConfiguration(new OrphanEventConfiguration());
        modelBuilder.ApplyConfiguration(new SponsorshipConfiguration());
    }

    /// <summary>
    /// Override SaveChanges to automatically set audit timestamps
    /// </summary>
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateAuditFields();
        return base.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Automatically set CreatedAt and UpdatedAt for entities
    /// </summary>
    private void UpdateAuditFields()
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.Entity is Domain.Common.BaseEntity &&
                       (e.State == EntityState.Added || e.State == EntityState.Modified));

        foreach (var entry in entries)
        {
            var entity = (Domain.Common.BaseEntity)entry.Entity;
            var now = DateTime.UtcNow;

            if (entry.State == EntityState.Added)
            {
                entity.CreatedAt = now;
            }

            entity.UpdatedAt = now;
        }
    }
}
