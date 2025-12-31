using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrphanManagement.Application.DTOs.Common;
using OrphanManagement.Application.DTOs.Events;
using OrphanManagement.Application.Interfaces.Services;
using OrphanManagement.Domain.Enums;

namespace OrphanManagement.API.Controllers;

/// <summary>
/// Controller for event management operations (CRUD, participant management, statistics)
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EventsController : ControllerBase
{
    private readonly IEventService _eventService;
    private readonly ILogger<EventsController> _logger;

    public EventsController(IEventService eventService, ILogger<EventsController> logger)
    {
        _eventService = eventService;
        _logger = logger;
    }

    /// <summary>
    /// Get all events with pagination and filtering
    /// </summary>
    /// <param name="pageNumber">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 10)</param>
    /// <param name="searchTerm">Search by title or description</param>
    /// <param name="eventType">Filter by event type</param>
    /// <param name="startDate">Filter by start date (from)</param>
    /// <param name="endDate">Filter by end date (to)</param>
    /// <param name="location">Filter by location</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of events</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<EventDto>>), 200)]
    public async Task<ActionResult<ApiResponse<PagedResult<EventDto>>>> GetEvents(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? searchTerm = null,
        [FromQuery] EventType? eventType = null,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        [FromQuery] string? location = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting events - Page: {PageNumber}, Size: {PageSize}", pageNumber, pageSize);

        var result = await _eventService.GetEventsAsync(
            pageNumber,
            pageSize,
            searchTerm,
            eventType,
            startDate,
            endDate,
            location,
            cancellationToken);

        return Ok(ApiResponse<PagedResult<EventDto>>.SuccessResponse(result));
    }

    /// <summary>
    /// Get event by ID
    /// </summary>
    /// <param name="id">Event ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Event details</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<EventDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public async Task<ActionResult<ApiResponse<EventDto>>> GetEventById(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting event by ID: {EventId}", id);

        var eventDto = await _eventService.GetEventByIdAsync(id, cancellationToken);

        if (eventDto == null)
        {
            _logger.LogWarning("Event not found: {EventId}", id);
            return NotFound(ApiResponse<object>.ErrorResponse("Event not found"));
        }

        return Ok(ApiResponse<EventDto>.SuccessResponse(eventDto));
    }

    /// <summary>
    /// Create a new event
    /// </summary>
    /// <param name="request">Event creation data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created event</returns>
    [HttpPost]
    [Authorize(Roles = "Admin,SocialWorker")]
    [ProducesResponseType(typeof(ApiResponse<EventDto>), 201)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    public async Task<ActionResult<ApiResponse<EventDto>>> CreateEvent(
        [FromBody] CreateEventRequest request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating new event: {Title}", request.Title);

        try
        {
            var eventDto = await _eventService.CreateEventAsync(request, cancellationToken);
            
            _logger.LogInformation("Event created successfully: {EventId}", eventDto.Id);
            return CreatedAtAction(
                nameof(GetEventById),
                new { id = eventDto.Id },
                ApiResponse<EventDto>.SuccessResponse(eventDto, "Event created successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating event");
            return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// Update an existing event
    /// </summary>
    /// <param name="id">Event ID</param>
    /// <param name="request">Event update data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated event</returns>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,SocialWorker")]
    [ProducesResponseType(typeof(ApiResponse<EventDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    public async Task<ActionResult<ApiResponse<EventDto>>> UpdateEvent(
        Guid id,
        [FromBody] UpdateEventRequest request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating event: {EventId}", id);

        try
        {
            var eventDto = await _eventService.UpdateEventAsync(id, request, cancellationToken);
            
            _logger.LogInformation("Event updated successfully: {EventId}", id);
            return Ok(ApiResponse<EventDto>.SuccessResponse(eventDto, "Event updated successfully"));
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning("Event not found for update: {EventId}", id);
            return NotFound(ApiResponse<object>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating event: {EventId}", id);
            return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// Delete an event
    /// </summary>
    /// <param name="id">Event ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>No content</returns>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(204)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public async Task<IActionResult> DeleteEvent(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting event: {EventId}", id);

        try
        {
            await _eventService.DeleteEventAsync(id, cancellationToken);
            
            _logger.LogInformation("Event deleted successfully: {EventId}", id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning("Event not found for deletion: {EventId}", id);
            return NotFound(ApiResponse<object>.ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// Add orphans to an event
    /// </summary>
    /// <param name="id">Event ID</param>
    /// <param name="orphanIds">List of orphan IDs to add</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success message</returns>
    [HttpPost("{id}/participants")]
    [Authorize(Roles = "Admin,SocialWorker")]
    [ProducesResponseType(typeof(ApiResponse<object>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    public async Task<ActionResult<ApiResponse<object>>> AddOrphansToEvent(
        Guid id,
        [FromBody] List<Guid> orphanIds,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Adding {Count} orphans to event: {EventId}", orphanIds.Count, id);

        try
        {
            await _eventService.AddOrphansToEventAsync(id, orphanIds, cancellationToken);
            
            _logger.LogInformation("Orphans added successfully to event: {EventId}", id);
            return Ok(ApiResponse<object>.SuccessResponse(null, "Orphans added to event successfully"));
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning("Event or orphans not found: {EventId}", id);
            return NotFound(ApiResponse<object>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding orphans to event: {EventId}", id);
            return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// Remove orphan from event
    /// </summary>
    /// <param name="id">Event ID</param>
    /// <param name="orphanId">Orphan ID to remove</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>No content</returns>
    [HttpDelete("{id}/participants/{orphanId}")]
    [Authorize(Roles = "Admin,SocialWorker")]
    [ProducesResponseType(204)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public async Task<IActionResult> RemoveOrphanFromEvent(
        Guid id,
        Guid orphanId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Removing orphan {OrphanId} from event: {EventId}", orphanId, id);

        try
        {
            await _eventService.RemoveOrphanFromEventAsync(id, orphanId, cancellationToken);
            
            _logger.LogInformation("Orphan removed successfully from event: {EventId}", id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning("Event or orphan not found: {EventId}, {OrphanId}", id, orphanId);
            return NotFound(ApiResponse<object>.ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// Update attendance status for an orphan in an event
    /// </summary>
    /// <param name="id">Event ID</param>
    /// <param name="orphanId">Orphan ID</param>
    /// <param name="request">Attendance update data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success message</returns>
    [HttpPatch("{id}/participants/{orphanId}/attendance")]
    [Authorize(Roles = "Admin,SocialWorker,Volunteer")]
    [ProducesResponseType(typeof(ApiResponse<object>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public async Task<ActionResult<ApiResponse<object>>> UpdateAttendance(
        Guid id,
        Guid orphanId,
        [FromBody] UpdateAttendanceRequest request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating attendance for orphan {OrphanId} in event: {EventId}", orphanId, id);

        try
        {
            await _eventService.UpdateAttendanceAsync(id, orphanId, request.Status, request.Notes, cancellationToken);
            
            _logger.LogInformation("Attendance updated successfully for event: {EventId}", id);
            return Ok(ApiResponse<object>.SuccessResponse(null, "Attendance updated successfully"));
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning("Event or orphan not found: {EventId}, {OrphanId}", id, orphanId);
            return NotFound(ApiResponse<object>.ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// Get upcoming events
    /// </summary>
    /// <param name="count">Number of events to return (default: 10)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of upcoming events</returns>
    [HttpGet("upcoming")]
    [ProducesResponseType(typeof(ApiResponse<List<EventDto>>), 200)]
    public async Task<ActionResult<ApiResponse<List<EventDto>>>> GetUpcomingEvents(
        [FromQuery] int count = 10,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting {Count} upcoming events", count);

        var events = await _eventService.GetUpcomingEventsAsync(count, cancellationToken);
        return Ok(ApiResponse<List<EventDto>>.SuccessResponse(events));
    }

    /// <summary>
    /// Get event statistics by month for a specific year
    /// </summary>
    /// <param name="year">Year for statistics</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Dictionary of months and event counts</returns>
    [HttpGet("statistics/by-month")]
    [ProducesResponseType(typeof(ApiResponse<Dictionary<string, int>>), 200)]
    public async Task<ActionResult<ApiResponse<Dictionary<string, int>>>> GetStatsByMonth(
        [FromQuery] int year = 0,
        CancellationToken cancellationToken = default)
    {
        if (year == 0)
        {
            year = DateTime.UtcNow.Year;
        }

        _logger.LogInformation("Getting event statistics by month for year: {Year}", year);

        var stats = await _eventService.GetEventStatsByMonthAsync(year, cancellationToken);
        return Ok(ApiResponse<Dictionary<string, int>>.SuccessResponse(stats));
    }
}

/// <summary>
/// DTO for updating attendance
/// </summary>
public class UpdateAttendanceRequest
{
    public AttendanceStatus Status { get; set; }
    public string? Notes { get; set; }
}
