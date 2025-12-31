namespace OrphanManagement.Domain.Enums;

/// <summary>
/// Defines the sponsorship status of an orphan
/// </summary>
public enum SponsorshipStatus
{
    /// <summary>
    /// Orphan is not currently sponsored
    /// </summary>
    NotSponsored = 1,
    
    /// <summary>
    /// Orphan is partially sponsored (some but not all needs covered)
    /// </summary>
    PartiallySponsored = 2,
    
    /// <summary>
    /// Orphan is fully sponsored (all needs covered)
    /// </summary>
    FullySponsored = 3
}
