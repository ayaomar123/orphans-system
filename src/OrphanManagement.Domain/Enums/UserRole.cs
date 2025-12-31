namespace OrphanManagement.Domain.Enums;

/// <summary>
/// Defines the available user roles in the system.
/// Each role has different permissions and access levels.
/// </summary>
public enum UserRole
{
    /// <summary>
    /// Full system access - can manage users, orphans, events, and all settings
    /// </summary>
    Admin = 1,
    
    /// <summary>
    /// Can manage orphans and events, but not users
    /// </summary>
    SocialWorker = 2,
    
    /// <summary>
    /// Can view and participate in events, limited orphan access
    /// </summary>
    Volunteer = 3,
    
    /// <summary>
    /// Read-only access to approved information
    /// </summary>
    Viewer = 4
}
