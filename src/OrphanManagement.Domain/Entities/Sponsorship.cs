using OrphanManagement.Domain.Common;
using OrphanManagement.Domain.Enums;

namespace OrphanManagement.Domain.Entities;

/// <summary>
/// Represents a sponsorship arrangement for an orphan.
/// Tracks financial support provided by sponsors.
/// </summary>
public class Sponsorship : BaseEntity
{
    /// <summary>
    /// Foreign key to the Orphan being sponsored
    /// </summary>
    public Guid OrphanId { get; set; }
    
    /// <summary>
    /// Navigation property to the Orphan entity
    /// </summary>
    public Orphan Orphan { get; set; } = null!;
    
    /// <summary>
    /// Name of the sponsor (can be individual or organization)
    /// </summary>
    public string SponsorName { get; set; } = string.Empty;
    
    /// <summary>
    /// Contact email for the sponsor
    /// </summary>
    public string? SponsorEmail { get; set; }
    
    /// <summary>
    /// Contact phone for the sponsor
    /// </summary>
    public string? SponsorPhone { get; set; }
    
    /// <summary>
    /// Amount of financial support
    /// </summary>
    public decimal Amount { get; set; }
    
    /// <summary>
    /// Currency code (e.g., USD, EUR, SAR)
    /// </summary>
    public string Currency { get; set; } = "USD";
    
    /// <summary>
    /// How often the sponsorship payment is made
    /// </summary>
    public SponsorshipFrequency Frequency { get; set; }
    
    /// <summary>
    /// Start date of the sponsorship
    /// </summary>
    public DateTime StartDate { get; set; }
    
    /// <summary>
    /// End date of the sponsorship (null if ongoing)
    /// </summary>
    public DateTime? EndDate { get; set; }
    
    /// <summary>
    /// Whether this sponsorship is currently active
    /// </summary>
    public bool IsActive { get; set; } = true;
    
    /// <summary>
    /// Additional notes about the sponsorship
    /// </summary>
    public string? Notes { get; set; }
    
    /// <summary>
    /// Computed property: Check if sponsorship is currently active based on dates
    /// </summary>
    public bool IsCurrentlyActive
    {
        get
        {
            var now = DateTime.UtcNow;
            return IsActive && StartDate <= now && (!EndDate.HasValue || EndDate.Value >= now);
        }
    }
}
