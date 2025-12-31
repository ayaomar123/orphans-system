using Microsoft.EntityFrameworkCore;
using OrphanManagement.Application.DTOs.Common;
using OrphanManagement.Application.Interfaces.Repositories;
using OrphanManagement.Domain.Entities;
using OrphanManagement.Domain.Enums;
using OrphanManagement.Infrastructure.Data;

namespace OrphanManagement.Infrastructure.Repositories;

public class EventRepository : Repository<Event>, IEventRepository
{
    public EventRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<PagedResult<Event>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        string? searchTerm = null,
        EventType? eventType = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        string? location = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var search = searchTerm.ToLower();
            query = query.Where(e => 
                e.Title.ToLower().Contains(search) ||
                (e.Description != null && e.Description.ToLower().Contains(search)));
        }

        if (eventType.HasValue)
        {
            query = query.Where(e => e.EventType == eventType.Value);
        }

        if (startDate.HasValue)
        {
            query = query.Where(e => e.StartDate >= startDate.Value);
        }

        if (endDate.HasValue)
        {
            query = query.Where(e => e.EndDate <= endDate.Value);
        }

        if (!string.IsNullOrWhiteSpace(location))
        {
            query = query.Where(e => e.Location.Contains(location));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Include(e => e.OrphanEvents)
            .OrderBy(e => e.StartDate)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Event>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public async Task<Event?> GetByIdWithParticipantsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(e => e.OrphanEvents)
                .ThenInclude(oe => oe.Orphan)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public async Task<List<Event>> GetUpcomingEventsAsync(int count = 10, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(e => e.StartDate > DateTime.UtcNow)
            .OrderBy(e => e.StartDate)
            .Take(count)
            .ToListAsync(cancellationToken);
    }

    public async Task AddOrphanToEventAsync(Guid eventId, Guid orphanId, CancellationToken cancellationToken = default)
    {
        var orphanEvent = new OrphanEvent
        {
            EventId = eventId,
            OrphanId = orphanId,
            AttendanceStatus = AttendanceStatus.Registered,
            RegisteredAt = DateTime.UtcNow
        };

        await _context.OrphanEvents.AddAsync(orphanEvent, cancellationToken);
    }

    public async Task RemoveOrphanFromEventAsync(Guid eventId, Guid orphanId, CancellationToken cancellationToken = default)
    {
        var orphanEvent = await _context.OrphanEvents
            .FirstOrDefaultAsync(oe => oe.EventId == eventId && oe.OrphanId == orphanId, cancellationToken);

        if (orphanEvent != null)
        {
            _context.OrphanEvents.Remove(orphanEvent);
        }
    }

    public async Task UpdateAttendanceAsync(Guid eventId, Guid orphanId, AttendanceStatus status, string? notes = null, CancellationToken cancellationToken = default)
    {
        var orphanEvent = await _context.OrphanEvents
            .FirstOrDefaultAsync(oe => oe.EventId == eventId && oe.OrphanId == orphanId, cancellationToken);

        if (orphanEvent != null)
        {
            orphanEvent.AttendanceStatus = status;
            orphanEvent.Notes = notes;
        }
    }

    public async Task<Dictionary<string, int>> GetEventCountByMonthAsync(int year, CancellationToken cancellationToken = default)
    {
        var events = await _dbSet
            .Where(e => e.StartDate.Year == year)
            .ToListAsync(cancellationToken);

        var monthlyCount = events
            .GroupBy(e => e.StartDate.ToString("MMM"))
            .ToDictionary(g => g.Key, g => g.Count());

        return monthlyCount;
    }
}
