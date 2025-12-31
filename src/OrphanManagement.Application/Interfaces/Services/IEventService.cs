using OrphanManagement.Application.DTOs.Common;
using OrphanManagement.Application.DTOs.Events;
using OrphanManagement.Domain.Enums;

namespace OrphanManagement.Application.Interfaces.Services;

/// <summary>
/// Service interface for event management operations
/// </summary>
public interface IEventService
{
    /// <summary>
    /// Get events with pagination and filters
    /// </summary>
    Task<PagedResult<EventDto>> GetEventsAsync(
        int pageNumber,
        int pageSize,
        string? searchTerm = null,
        EventType? eventType = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        string? location = null,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get event by ID with participants
    /// </summary>
    Task<EventDto?> GetEventByIdAsync(Guid id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Create a new event
    /// </summary>
    Task<EventDto> CreateEventAsync(CreateEventRequest request, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Update an existing event
    /// </summary>
    Task<EventDto> UpdateEventAsync(Guid id, UpdateEventRequest request, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Delete an event
    /// </summary>
    Task DeleteEventAsync(Guid id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Add orphans to an event
    /// </summary>
    Task AddOrphansToEventAsync(Guid eventId, List<Guid> orphanIds, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Remove orphan from event
    /// </summary>
    Task RemoveOrphanFromEventAsync(Guid eventId, Guid orphanId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Update attendance status
    /// </summary>
    Task UpdateAttendanceAsync(Guid eventId, Guid orphanId, AttendanceStatus status, string? notes = null, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get upcoming events
    /// </summary>
    Task<List<EventDto>> GetUpcomingEventsAsync(int count = 10, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get event statistics by month
    /// </summary>
    Task<Dictionary<string, int>> GetEventStatsByMonthAsync(int year, CancellationToken cancellationToken = default);
}
