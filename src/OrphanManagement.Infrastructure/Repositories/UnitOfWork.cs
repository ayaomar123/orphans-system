using Microsoft.EntityFrameworkCore.Storage;
using OrphanManagement.Application.Interfaces.Repositories;
using OrphanManagement.Domain.Entities;
using OrphanManagement.Infrastructure.Data;

namespace OrphanManagement.Infrastructure.Repositories;

/// <summary>
/// Unit of Work implementation that coordinates database transactions
/// across multiple repositories.
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IDbContextTransaction? _transaction;

    public IRepository<User> Users { get; }
    public IOrphanRepository Orphans { get; }
    public IEventRepository Events { get; }
    public IRepository<Sponsorship> Sponsorships { get; }

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        
        Users = new Repository<User>(context);
        Orphans = new OrphanRepository(context);
        Events = new EventRepository(context);
        Sponsorships = new Repository<Sponsorship>(context);
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}
