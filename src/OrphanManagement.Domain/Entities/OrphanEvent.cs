using OrphanManagement.Domain.Enums;

namespace OrphanManagement.Domain.Entities;

/// <summary>
/// Represents the many-to-many relationship between Orphans and Events.
/// Tracks which orphans are participating in which events and their attendance status.
/// </summary>
public class OrphanEvent
{
    /// <summary>
    /// Foreign key to the Orphan entity
    /// </summary>
    public Guid OrphanId { get; set; }
    
    /// <summary>
    /// Navigation property to the Orphan entity
    /// </summary>
    public Orphan Orphan { get; set; } = null!;
    
    /// <summary>
    /// Foreign key to the Event entity
    /// </summary>
    public Guid EventId { get; set; }
    
    /// <summary>
    /// Navigation property to the Event entity
    /// </summary>
    public Event Event { get; set; } = null!;
    
    /// <summary>
    /// Attendance status for this orphan at this event
    /// </summary>
    public AttendanceStatus AttendanceStatus { get; set; } = AttendanceStatus.Registered;
    
    /// <summary>
    /// Additional notes about the orphan's participation in this event
    /// </summary>
    public string? Notes { get; set; }
    
    /// <summary>
    /// When the orphan was registered for this event
    /// </summary>
    public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;
}
