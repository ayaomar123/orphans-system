using OrphanManagement.Application.DTOs.Auth;

namespace OrphanManagement.Application.Interfaces.Services;

/// <summary>
/// Service interface for authentication operations
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Authenticate user and generate JWT token
    /// </summary>
    Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Validate a JWT token
    /// </summary>
    Task<bool> ValidateTokenAsync(string token);
    
    /// <summary>
    /// Hash a password using BCrypt
    /// </summary>
    string HashPassword(string password);
    
    /// <summary>
    /// Verify a password against a hash
    /// </summary>
    bool VerifyPassword(string password, string passwordHash);
}
