using OrphanManagement.Application.DTOs.Common;
using OrphanManagement.Domain.Entities;
using OrphanManagement.Domain.Enums;

namespace OrphanManagement.Application.Interfaces.Repositories;

/// <summary>
/// Extended repository interface for Event entity with custom query methods
/// </summary>
public interface IEventRepository : IRepository<Event>
{
    /// <summary>
    /// Get paginated and filtered list of events
    /// </summary>
    Task<PagedResult<Event>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        string? searchTerm = null,
        EventType? eventType = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        string? location = null,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get event with all participants
    /// </summary>
    Task<Event?> GetByIdWithParticipantsAsync(Guid id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get upcoming events
    /// </summary>
    Task<List<Event>> GetUpcomingEventsAsync(int count = 10, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Add orphan to event
    /// </summary>
    Task AddOrphanToEventAsync(Guid eventId, Guid orphanId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Remove orphan from event
    /// </summary>
    Task RemoveOrphanFromEventAsync(Guid eventId, Guid orphanId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Update orphan attendance status
    /// </summary>
    Task UpdateAttendanceAsync(Guid eventId, Guid orphanId, AttendanceStatus status, string? notes = null, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get event statistics per month
    /// </summary>
    Task<Dictionary<string, int>> GetEventCountByMonthAsync(int year, CancellationToken cancellationToken = default);
}
