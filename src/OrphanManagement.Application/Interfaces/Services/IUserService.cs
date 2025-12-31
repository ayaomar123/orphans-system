using OrphanManagement.Application.DTOs.Common;
using OrphanManagement.Application.DTOs.Users;

namespace OrphanManagement.Application.Interfaces.Services;

/// <summary>
/// Service interface for user management operations
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Get all users with pagination
    /// </summary>
    Task<PagedResult<UserDto>> GetUsersAsync(int pageNumber, int pageSize, string? searchTerm = null, string? role = null, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get user by ID
    /// </summary>
    Task<UserDto?> GetUserByIdAsync(Guid id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get user by email
    /// </summary>
    Task<UserDto?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Create a new user
    /// </summary>
    Task<UserDto> CreateUserAsync(CreateUserRequest request, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Update an existing user
    /// </summary>
    Task<UserDto> UpdateUserAsync(Guid id, UpdateUserRequest request, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Delete a user
    /// </summary>
    Task DeleteUserAsync(Guid id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Activate or deactivate a user account
    /// </summary>
    Task ToggleUserStatusAsync(Guid id, bool isActive, CancellationToken cancellationToken = default);
}
