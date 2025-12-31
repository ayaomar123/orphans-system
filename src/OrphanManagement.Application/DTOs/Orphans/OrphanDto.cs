namespace OrphanManagement.Application.DTOs.Orphans;

/// <summary>
/// DTO for orphan information (read operations)
/// </summary>
public class OrphanDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public int Age { get; set; }
    public string? NationalId { get; set; }
    public string City { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string HealthStatus { get; set; } = string.Empty;
    public string? HealthNotes { get; set; }
    public string EducationStatus { get; set; } = string.Empty;
    public string? SchoolName { get; set; }
    public string? ClassLevel { get; set; }
    public string? GuardianName { get; set; }
    public string? GuardianPhone { get; set; }
    public string? GuardianRelationship { get; set; }
    public string SponsorshipStatus { get; set; } = string.Empty;
    public string? PhotoUrl { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
