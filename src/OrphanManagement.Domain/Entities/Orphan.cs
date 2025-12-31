using OrphanManagement.Domain.Common;
using OrphanManagement.Domain.Enums;

namespace OrphanManagement.Domain.Entities;

/// <summary>
/// Represents an orphan in the system with complete profile information.
/// Includes personal details, education, health, guardian info, and sponsorship status.
/// </summary>
public class Orphan : BaseEntity
{
    /// <summary>
    /// First name of the orphan
    /// </summary>
    public string FirstName { get; set; } = string.Empty;
    
    /// <summary>
    /// Last name of the orphan
    /// </summary>
    public string LastName { get; set; } = string.Empty;
    
    /// <summary>
    /// Gender of the orphan
    /// </summary>
    public Gender Gender { get; set; }
    
    /// <summary>
    /// Date of birth
    /// </summary>
    public DateTime DateOfBirth { get; set; }
    
    /// <summary>
    /// National ID number (unique identifier)
    /// </summary>
    public string? NationalId { get; set; }
    
    /// <summary>
    /// City where the orphan resides
    /// </summary>
    public string City { get; set; } = string.Empty;
    
    /// <summary>
    /// Full residential address
    /// </summary>
    public string? Address { get; set; }
    
    /// <summary>
    /// Current health status
    /// </summary>
    public HealthStatus HealthStatus { get; set; }
    
    /// <summary>
    /// Additional health notes or medical conditions
    /// </summary>
    public string? HealthNotes { get; set; }
    
    /// <summary>
    /// Current education status
    /// </summary>
    public EducationStatus EducationStatus { get; set; }
    
    /// <summary>
    /// Name of the school currently attending (if applicable)
    /// </summary>
    public string? SchoolName { get; set; }
    
    /// <summary>
    /// Current class/grade level
    /// </summary>
    public string? ClassLevel { get; set; }
    
    /// <summary>
    /// Name of the legal guardian
    /// </summary>
    public string? GuardianName { get; set; }
    
    /// <summary>
    /// Guardian's contact phone number
    /// </summary>
    public string? GuardianPhone { get; set; }
    
    /// <summary>
    /// Relationship of guardian to the orphan (e.g., Uncle, Aunt, Grandparent)
    /// </summary>
    public string? GuardianRelationship { get; set; }
    
    /// <summary>
    /// Current sponsorship status
    /// </summary>
    public SponsorshipStatus SponsorshipStatus { get; set; }
    
    /// <summary>
    /// URL to the orphan's profile photo
    /// </summary>
    public string? PhotoUrl { get; set; }
    
    /// <summary>
    /// Additional notes about the orphan
    /// </summary>
    public string? Notes { get; set; }
    
    // Navigation properties
    
    /// <summary>
    /// List of sponsorships for this orphan
    /// </summary>
    public ICollection<Sponsorship> Sponsorships { get; set; } = new List<Sponsorship>();
    
    /// <summary>
    /// List of event participations
    /// </summary>
    public ICollection<OrphanEvent> OrphanEvents { get; set; } = new List<OrphanEvent>();
    
    /// <summary>
    /// Computed property: Full name of the orphan
    /// </summary>
    public string FullName => $"{FirstName} {LastName}";
    
    /// <summary>
    /// Computed property: Current age in years
    /// </summary>
    public int Age => DateTime.UtcNow.Year - DateOfBirth.Year - 
                      (DateTime.UtcNow.DayOfYear < DateOfBirth.DayOfYear ? 1 : 0);
}
