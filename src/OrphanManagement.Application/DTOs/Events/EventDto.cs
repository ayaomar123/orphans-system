namespace OrphanManagement.Application.DTOs.Events;

/// <summary>
/// DTO for event information (read operations)
/// </summary>
public class EventDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Location { get; set; } = string.Empty;
    public string EventType { get; set; } = string.Empty;
    public int? MaxParticipants { get; set; }
    public decimal? Budget { get; set; }
    public string? Notes { get; set; }
    public int ParticipantCount { get; set; }
    public bool IsUpcoming { get; set; }
    public bool IsOngoing { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
