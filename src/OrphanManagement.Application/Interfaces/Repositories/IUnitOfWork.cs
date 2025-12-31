using OrphanManagement.Domain.Entities;

namespace OrphanManagement.Application.Interfaces.Repositories;

/// <summary>
/// Unit of Work pattern interface.
/// Coordinates the work of multiple repositories and ensures transactions are atomic.
/// </summary>
public interface IUnitOfWork : IDisposable
{
    /// <summary>
    /// Repository for User entities
    /// </summary>
    IRepository<User> Users { get; }
    
    /// <summary>
    /// Repository for Orphan entities
    /// </summary>
    IOrphanRepository Orphans { get; }
    
    /// <summary>
    /// Repository for Event entities
    /// </summary>
    IEventRepository Events { get; }
    
    /// <summary>
    /// Repository for Sponsorship entities
    /// </summary>
    IRepository<Sponsorship> Sponsorships { get; }
    
    /// <summary>
    /// Save all changes to the database
    /// </summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Begin a database transaction
    /// </summary>
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Commit the current transaction
    /// </summary>
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Rollback the current transaction
    /// </summary>
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
