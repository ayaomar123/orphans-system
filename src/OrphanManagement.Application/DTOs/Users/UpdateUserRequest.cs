using OrphanManagement.Domain.Enums;

namespace OrphanManagement.Application.DTOs.Users;

/// <summary>
/// DTO for updating an existing user
/// </summary>
public class UpdateUserRequest
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public string? Phone { get; set; }
    public bool IsActive { get; set; }
}
