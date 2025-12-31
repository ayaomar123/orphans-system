namespace OrphanManagement.Domain.Enums;

/// <summary>
/// Defines the general health status of an orphan
/// </summary>
public enum HealthStatus
{
    /// <summary>
    /// Healthy with no known medical conditions
    /// </summary>
    Healthy = 1,
    
    /// <summary>
    /// Minor health issues that don't require ongoing treatment
    /// </summary>
    MinorIssues = 2,
    
    /// <summary>
    /// Chronic condition requiring ongoing care or treatment
    /// </summary>
    ChronicCondition = 3,
    
    /// <summary>
    /// Requires special medical attention or care
    /// </summary>
    SpecialNeeds = 4
}
