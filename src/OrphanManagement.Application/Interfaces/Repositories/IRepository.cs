using OrphanManagement.Domain.Common;
using System.Linq.Expressions;

namespace OrphanManagement.Application.Interfaces.Repositories;

/// <summary>
/// Generic repository interface for common CRUD operations.
/// Provides abstraction over data access layer.
/// </summary>
/// <typeparam name="T">Entity type that inherits from BaseEntity</typeparam>
public interface IRepository<T> where T : BaseEntity
{
    /// <summary>
    /// Get entity by ID
    /// </summary>
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get all entities
    /// </summary>
    Task<List<T>> GetAllAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Find entities matching a predicate
    /// </summary>
    Task<List<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get first entity matching a predicate
    /// </summary>
    Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Add a new entity
    /// </summary>
    Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Add multiple entities
    /// </summary>
    Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Update an existing entity
    /// </summary>
    void Update(T entity);
    
    /// <summary>
    /// Delete an entity
    /// </summary>
    void Delete(T entity);
    
    /// <summary>
    /// Delete multiple entities
    /// </summary>
    void DeleteRange(IEnumerable<T> entities);
    
    /// <summary>
    /// Check if any entity matches the predicate
    /// </summary>
    Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Count entities matching the predicate
    /// </summary>
    Task<int> CountAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
}
