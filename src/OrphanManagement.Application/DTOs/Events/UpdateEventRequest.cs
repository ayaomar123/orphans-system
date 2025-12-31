using OrphanManagement.Domain.Enums;

namespace OrphanManagement.Application.DTOs.Events;

/// <summary>
/// DTO for updating an existing event
/// </summary>
public class UpdateEventRequest
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Location { get; set; } = string.Empty;
    public EventType EventType { get; set; }
    public int? MaxParticipants { get; set; }
    public decimal? Budget { get; set; }
    public string? Notes { get; set; }
}
