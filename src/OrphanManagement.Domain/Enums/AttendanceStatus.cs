namespace OrphanManagement.Domain.Enums;

/// <summary>
/// Defines the attendance status for an orphan at an event
/// </summary>
public enum AttendanceStatus
{
    /// <summary>
    /// Registered but attendance not yet confirmed
    /// </summary>
    Registered = 1,
    
    /// <summary>
    /// Confirmed attendance at the event
    /// </summary>
    Attended = 2,
    
    /// <summary>
    /// Did not attend the event
    /// </summary>
    Absent = 3,
    
    /// <summary>
    /// Cancelled participation before the event
    /// </summary>
    Cancelled = 4
}
