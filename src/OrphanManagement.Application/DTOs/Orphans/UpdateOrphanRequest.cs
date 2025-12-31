using OrphanManagement.Domain.Enums;

namespace OrphanManagement.Application.DTOs.Orphans;

/// <summary>
/// DTO for updating an existing orphan record
/// </summary>
public class UpdateOrphanRequest
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public Gender Gender { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string? NationalId { get; set; }
    public string City { get; set; } = string.Empty;
    public string? Address { get; set; }
    public HealthStatus HealthStatus { get; set; }
    public string? HealthNotes { get; set; }
    public EducationStatus EducationStatus { get; set; }
    public string? SchoolName { get; set; }
    public string? ClassLevel { get; set; }
    public string? GuardianName { get; set; }
    public string? GuardianPhone { get; set; }
    public string? GuardianRelationship { get; set; }
    public SponsorshipStatus SponsorshipStatus { get; set; }
    public string? Notes { get; set; }
}
