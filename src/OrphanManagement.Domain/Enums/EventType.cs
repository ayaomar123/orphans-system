namespace OrphanManagement.Domain.Enums;

/// <summary>
/// Defines different types of events that can be organized for orphans
/// </summary>
public enum EventType
{
    /// <summary>
    /// Educational workshop or training session
    /// </summary>
    Educational = 1,
    
    /// <summary>
    /// Recreational activity or outing
    /// </summary>
    Recreational = 2,
    
    /// <summary>
    /// Medical checkup or health screening
    /// </summary>
    Medical = 3,
    
    /// <summary>
    /// Cultural or religious event
    /// </summary>
    Cultural = 4,
    
    /// <summary>
    /// Sports activity or competition
    /// </summary>
    Sports = 5,
    
    /// <summary>
    /// Arts and crafts workshop
    /// </summary>
    Arts = 6,
    
    /// <summary>
    /// Community service or volunteering activity
    /// </summary>
    CommunityService = 7,
    
    /// <summary>
    /// Other type of event
    /// </summary>
    Other = 8
}
