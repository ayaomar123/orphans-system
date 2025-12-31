namespace OrphanManagement.Domain.Enums;

/// <summary>
/// Defines how often sponsorship payments are made
/// </summary>
public enum SponsorshipFrequency
{
    /// <summary>
    /// One-time payment
    /// </summary>
    OneTime = 1,
    
    /// <summary>
    /// Monthly recurring payment
    /// </summary>
    Monthly = 2,
    
    /// <summary>
    /// Quarterly recurring payment
    /// </summary>
    Quarterly = 3,
    
    /// <summary>
    /// Yearly recurring payment
    /// </summary>
    Yearly = 4
}
