using OrphanManagement.Domain.Common;
using OrphanManagement.Domain.Enums;

namespace OrphanManagement.Domain.Entities;

/// <summary>
/// Represents an event or activity organized for orphans.
/// Events can be educational, recreational, medical, cultural, etc.
/// </summary>
public class Event : BaseEntity
{
    /// <summary>
    /// Title of the event
    /// </summary>
    public string Title { get; set; } = string.Empty;
    
    /// <summary>
    /// Detailed description of the event
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// Start date and time of the event
    /// </summary>
    public DateTime StartDate { get; set; }
    
    /// <summary>
    /// End date and time of the event
    /// </summary>
    public DateTime EndDate { get; set; }
    
    /// <summary>
    /// Location where the event will take place
    /// </summary>
    public string Location { get; set; } = string.Empty;
    
    /// <summary>
    /// Type/category of the event
    /// </summary>
    public EventType EventType { get; set; }
    
    /// <summary>
    /// Maximum number of participants allowed
    /// </summary>
    public int? MaxParticipants { get; set; }
    
    /// <summary>
    /// Allocated budget for the event
    /// </summary>
    public decimal? Budget { get; set; }
    
    /// <summary>
    /// Additional notes about the event
    /// </summary>
    public string? Notes { get; set; }
    
    // Navigation properties
    
    /// <summary>
    /// List of orphans participating in this event
    /// </summary>
    public ICollection<OrphanEvent> OrphanEvents { get; set; } = new List<OrphanEvent>();
    
    /// <summary>
    /// Computed property: Check if the event is upcoming
    /// </summary>
    public bool IsUpcoming => StartDate > DateTime.UtcNow;
    
    /// <summary>
    /// Computed property: Check if the event is ongoing
    /// </summary>
    public bool IsOngoing => StartDate <= DateTime.UtcNow && EndDate >= DateTime.UtcNow;
    
    /// <summary>
    /// Computed property: Check if the event is completed
    /// </summary>
    public bool IsCompleted => EndDate < DateTime.UtcNow;
    
    /// <summary>
    /// Computed property: Current number of participants
    /// </summary>
    public int ParticipantCount => OrphanEvents.Count;
    
    /// <summary>
    /// Computed property: Check if event is at capacity
    /// </summary>
    public bool IsFull => MaxParticipants.HasValue && ParticipantCount >= MaxParticipants.Value;
}
