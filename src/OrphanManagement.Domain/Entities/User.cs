using OrphanManagement.Domain.Common;
using OrphanManagement.Domain.Enums;

namespace OrphanManagement.Domain.Entities;

/// <summary>
/// Represents a user in the system (Admin, SocialWorker, Volunteer, or Viewer).
/// Handles authentication and authorization.
/// </summary>
public class User : BaseEntity
{
    /// <summary>
    /// Full name of the user
    /// </summary>
    public string FullName { get; set; } = string.Empty;
    
    /// <summary>
    /// Unique email address for login
    /// </summary>
    public string Email { get; set; } = string.Empty;
    
    /// <summary>
    /// Hashed password (never store plain text passwords)
    /// </summary>
    public string PasswordHash { get; set; } = string.Empty;
    
    /// <summary>
    /// User's role determining their permissions
    /// </summary>
    public UserRole Role { get; set; }
    
    /// <summary>
    /// Contact phone number
    /// </summary>
    public string? Phone { get; set; }
    
    /// <summary>
    /// Whether the user account is active or deactivated
    /// </summary>
    public bool IsActive { get; set; } = true;
    
    /// <summary>
    /// Last login timestamp for auditing
    /// </summary>
    public DateTime? LastLoginAt { get; set; }
}
