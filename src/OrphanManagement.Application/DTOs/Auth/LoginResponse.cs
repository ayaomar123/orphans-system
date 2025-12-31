namespace OrphanManagement.Application.DTOs.Auth;

/// <summary>
/// DTO for login response containing JWT token and user information
/// </summary>
public class LoginResponse
{
    /// <summary>
    /// JWT access token
    /// </summary>
    public string Token { get; set; } = string.Empty;
    
    /// <summary>
    /// Token expiration timestamp
    /// </summary>
    public DateTime ExpiresAt { get; set; }
    
    /// <summary>
    /// User ID
    /// </summary>
    public Guid UserId { get; set; }
    
    /// <summary>
    /// User's full name
    /// </summary>
    public string FullName { get; set; } = string.Empty;
    
    /// <summary>
    /// User's email
    /// </summary>
    public string Email { get; set; } = string.Empty;
    
    /// <summary>
    /// User's role
    /// </summary>
    public string Role { get; set; } = string.Empty;
}
