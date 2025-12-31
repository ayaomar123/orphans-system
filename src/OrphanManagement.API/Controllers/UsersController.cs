using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrphanManagement.Application.DTOs.Common;
using OrphanManagement.Application.DTOs.Users;
using OrphanManagement.Application.Interfaces.Services;

namespace OrphanManagement.API.Controllers;

/// <summary>
/// Controller for user management operations (CRUD, status management)
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UsersController> _logger;

    public UsersController(IUserService userService, ILogger<UsersController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    /// <summary>
    /// Get all users with pagination and filtering
    /// </summary>
    /// <param name="pageNumber">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 10)</param>
    /// <param name="searchTerm">Search by name or email</param>
    /// <param name="role">Filter by role</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of users</returns>
    [HttpGet]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<UserDto>>), 200)]
    public async Task<ActionResult<ApiResponse<PagedResult<UserDto>>>> GetUsers(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? searchTerm = null,
        [FromQuery] string? role = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting users - Page: {PageNumber}, Size: {PageSize}", pageNumber, pageSize);

        var result = await _userService.GetUsersAsync(pageNumber, pageSize, searchTerm, role, cancellationToken);
        return Ok(ApiResponse<PagedResult<UserDto>>.SuccessResponse(result));
    }

    /// <summary>
    /// Get user by ID
    /// </summary>
    /// <param name="id">User ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>User details</returns>
    [HttpGet("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public async Task<ActionResult<ApiResponse<UserDto>>> GetUserById(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting user by ID: {UserId}", id);

        var user = await _userService.GetUserByIdAsync(id, cancellationToken);

        if (user == null)
        {
            _logger.LogWarning("User not found: {UserId}", id);
            return NotFound(ApiResponse<object>.ErrorResponse("User not found"));
        }

        return Ok(ApiResponse<UserDto>.SuccessResponse(user));
    }

    /// <summary>
    /// Get user by email
    /// </summary>
    /// <param name="email">User email</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>User details</returns>
    [HttpGet("by-email/{email}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public async Task<ActionResult<ApiResponse<UserDto>>> GetUserByEmail(
        string email,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting user by email: {Email}", email);

        var user = await _userService.GetUserByEmailAsync(email, cancellationToken);

        if (user == null)
        {
            _logger.LogWarning("User not found: {Email}", email);
            return NotFound(ApiResponse<object>.ErrorResponse("User not found"));
        }

        return Ok(ApiResponse<UserDto>.SuccessResponse(user));
    }

    /// <summary>
    /// Create a new user
    /// </summary>
    /// <param name="request">User creation data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created user</returns>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), 201)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    public async Task<ActionResult<ApiResponse<UserDto>>> CreateUser(
        [FromBody] CreateUserRequest request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating new user: {Email}", request.Email);

        try
        {
            var user = await _userService.CreateUserAsync(request, cancellationToken);
            
            _logger.LogInformation("User created successfully: {UserId}", user.Id);
            return CreatedAtAction(
                nameof(GetUserById),
                new { id = user.Id },
                ApiResponse<UserDto>.SuccessResponse(user, "User created successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user");
            return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// Update an existing user
    /// </summary>
    /// <param name="id">User ID</param>
    /// <param name="request">User update data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated user</returns>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    public async Task<ActionResult<ApiResponse<UserDto>>> UpdateUser(
        Guid id,
        [FromBody] UpdateUserRequest request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating user: {UserId}", id);

        try
        {
            var user = await _userService.UpdateUserAsync(id, request, cancellationToken);
            
            _logger.LogInformation("User updated successfully: {UserId}", id);
            return Ok(ApiResponse<UserDto>.SuccessResponse(user, "User updated successfully"));
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning("User not found for update: {UserId}", id);
            return NotFound(ApiResponse<object>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user: {UserId}", id);
            return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// Delete a user
    /// </summary>
    /// <param name="id">User ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>No content</returns>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(204)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public async Task<IActionResult> DeleteUser(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting user: {UserId}", id);

        try
        {
            await _userService.DeleteUserAsync(id, cancellationToken);
            
            _logger.LogInformation("User deleted successfully: {UserId}", id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning("User not found for deletion: {UserId}", id);
            return NotFound(ApiResponse<object>.ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// Activate or deactivate a user account
    /// </summary>
    /// <param name="id">User ID</param>
    /// <param name="request">Status toggle request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success message</returns>
    [HttpPatch("{id}/status")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<object>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public async Task<ActionResult<ApiResponse<object>>> ToggleUserStatus(
        Guid id,
        [FromBody] ToggleUserStatusRequest request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Toggling status for user: {UserId} to {IsActive}", id, request.IsActive);

        try
        {
            await _userService.ToggleUserStatusAsync(id, request.IsActive, cancellationToken);
            
            var statusText = request.IsActive ? "activated" : "deactivated";
            _logger.LogInformation("User {Status} successfully: {UserId}", statusText, id);
            return Ok(ApiResponse<object>.SuccessResponse(null, $"User {statusText} successfully"));
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning("User not found for status toggle: {UserId}", id);
            return NotFound(ApiResponse<object>.ErrorResponse(ex.Message));
        }
    }
}

/// <summary>
/// DTO for toggling user status
/// </summary>
public class ToggleUserStatusRequest
{
    public bool IsActive { get; set; }
}
