using AutoMapper;
using Microsoft.Extensions.Logging;
using OrphanManagement.Application.DTOs.Common;
using OrphanManagement.Application.DTOs.Events;
using OrphanManagement.Application.Interfaces.Repositories;
using OrphanManagement.Application.Interfaces.Services;
using OrphanManagement.Domain.Entities;
using OrphanManagement.Domain.Enums;
using OrphanManagement.Domain.Exceptions;

namespace OrphanManagement.Infrastructure.Services;

public class EventService : IEventService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<EventService> _logger;

    public EventService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<EventService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<PagedResult<EventDto>> GetEventsAsync(
        int pageNumber,
        int pageSize,
        string? searchTerm = null,
        EventType? eventType = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        string? location = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting events with pagination. Page: {PageNumber}, Size: {PageSize}", pageNumber, pageSize);

        var pagedEvents = await _unitOfWork.Events.GetPagedAsync(
            pageNumber,
            pageSize,
            searchTerm,
            eventType,
            startDate,
            endDate,
            location,
            cancellationToken);

        var eventDtos = _mapper.Map<List<EventDto>>(pagedEvents.Items);

        return new PagedResult<EventDto>
        {
            Items = eventDtos,
            TotalCount = pagedEvents.TotalCount,
            PageNumber = pagedEvents.PageNumber,
            PageSize = pagedEvents.PageSize
        };
    }

    public async Task<EventDto?> GetEventByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting event by ID: {EventId}", id);

        var eventEntity = await _unitOfWork.Events.GetByIdWithParticipantsAsync(id, cancellationToken);
        
        if (eventEntity == null)
        {
            _logger.LogWarning("Event not found with ID: {EventId}", id);
            return null;
        }

        return _mapper.Map<EventDto>(eventEntity);
    }

    public async Task<EventDto> CreateEventAsync(CreateEventRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating new event: {Title}", request.Title);

        if (request.StartDate >= request.EndDate)
        {
            throw new DomainException("Event start date must be before end date.");
        }

        var eventEntity = _mapper.Map<Event>(request);
        eventEntity.CreatedAt = DateTime.UtcNow;
        eventEntity.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.Events.AddAsync(eventEntity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Event created successfully with ID: {EventId}", eventEntity.Id);

        return _mapper.Map<EventDto>(eventEntity);
    }

    public async Task<EventDto> UpdateEventAsync(Guid id, UpdateEventRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating event with ID: {EventId}", id);

        var eventEntity = await _unitOfWork.Events.GetByIdAsync(id, cancellationToken);
        
        if (eventEntity == null)
        {
            throw new NotFoundException("Event", id);
        }

        if (request.StartDate >= request.EndDate)
        {
            throw new DomainException("Event start date must be before end date.");
        }

        _mapper.Map(request, eventEntity);
        eventEntity.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Events.Update(eventEntity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Event updated successfully with ID: {EventId}", id);

        return _mapper.Map<EventDto>(eventEntity);
    }

    public async Task DeleteEventAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting event with ID: {EventId}", id);

        var eventEntity = await _unitOfWork.Events.GetByIdAsync(id, cancellationToken);
        
        if (eventEntity == null)
        {
            throw new NotFoundException("Event", id);
        }

        _unitOfWork.Events.Delete(eventEntity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Event deleted successfully with ID: {EventId}", id);
    }

    public async Task AddOrphansToEventAsync(Guid eventId, List<Guid> orphanIds, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Adding {Count} orphans to event with ID: {EventId}", orphanIds.Count, eventId);

        var eventEntity = await _unitOfWork.Events.GetByIdWithParticipantsAsync(eventId, cancellationToken);
        
        if (eventEntity == null)
        {
            throw new NotFoundException("Event", eventId);
        }

        if (eventEntity.IsFull)
        {
            throw new DomainException("Event has reached maximum capacity.");
        }

        foreach (var orphanId in orphanIds)
        {
            var orphan = await _unitOfWork.Orphans.GetByIdAsync(orphanId, cancellationToken);
            
            if (orphan == null)
            {
                throw new NotFoundException("Orphan", orphanId);
            }

            if (eventEntity.MaxParticipants.HasValue && 
                eventEntity.ParticipantCount >= eventEntity.MaxParticipants.Value)
            {
                _logger.LogWarning("Event {EventId} has reached maximum capacity. Stopping at {Count} orphans.", 
                    eventId, eventEntity.ParticipantCount);
                break;
            }

            await _unitOfWork.Events.AddOrphanToEventAsync(eventId, orphanId, cancellationToken);
        }

        _logger.LogInformation("Orphans added to event successfully");
    }

    public async Task RemoveOrphanFromEventAsync(Guid eventId, Guid orphanId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Removing orphan {OrphanId} from event {EventId}", orphanId, eventId);

        var eventEntity = await _unitOfWork.Events.GetByIdAsync(eventId, cancellationToken);
        
        if (eventEntity == null)
        {
            throw new NotFoundException("Event", eventId);
        }

        var orphan = await _unitOfWork.Orphans.GetByIdAsync(orphanId, cancellationToken);
        
        if (orphan == null)
        {
            throw new NotFoundException("Orphan", orphanId);
        }

        await _unitOfWork.Events.RemoveOrphanFromEventAsync(eventId, orphanId, cancellationToken);

        _logger.LogInformation("Orphan removed from event successfully");
    }

    public async Task UpdateAttendanceAsync(
        Guid eventId, 
        Guid orphanId, 
        AttendanceStatus status, 
        string? notes = null, 
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating attendance for orphan {OrphanId} at event {EventId}", orphanId, eventId);

        var eventEntity = await _unitOfWork.Events.GetByIdAsync(eventId, cancellationToken);
        
        if (eventEntity == null)
        {
            throw new NotFoundException("Event", eventId);
        }

        var orphan = await _unitOfWork.Orphans.GetByIdAsync(orphanId, cancellationToken);
        
        if (orphan == null)
        {
            throw new NotFoundException("Orphan", orphanId);
        }

        await _unitOfWork.Events.UpdateAttendanceAsync(eventId, orphanId, status, notes, cancellationToken);

        _logger.LogInformation("Attendance updated successfully");
    }

    public async Task<List<EventDto>> GetUpcomingEventsAsync(int count = 10, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting {Count} upcoming events", count);

        var events = await _unitOfWork.Events.GetUpcomingEventsAsync(count, cancellationToken);
        
        return _mapper.Map<List<EventDto>>(events);
    }

    public async Task<Dictionary<string, int>> GetEventStatsByMonthAsync(int year, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting event statistics by month for year {Year}", year);

        var stats = await _unitOfWork.Events.GetEventCountByMonthAsync(year, cancellationToken);

        return stats;
    }
}
