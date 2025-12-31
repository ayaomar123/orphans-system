using Microsoft.AspNetCore.Mvc;
using OrphanManagement.Application.DTOs.Auth;
using OrphanManagement.Application.DTOs.Common;
using OrphanManagement.Application.Interfaces.Services;

namespace OrphanManagement.API.Controllers;

/// <summary>
/// Controller for authentication operations (login, register, token management)
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    /// <summary>
    /// User login endpoint
    /// </summary>
    /// <param name="request">Login credentials</param>
    /// <returns>JWT token and user information</returns>
    [HttpPost("login")]
    [ProducesResponseType(typeof(ApiResponse<LoginResponse>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    public async Task<ActionResult<ApiResponse<LoginResponse>>> Login([FromBody] LoginRequest request)
    {
        _logger.LogInformation("Login attempt for email: {Email}", request.Email);

        try
        {
            var result = await _authService.LoginAsync(request);
            
            _logger.LogInformation("User {Email} logged in successfully", request.Email);
            return Ok(ApiResponse<LoginResponse>.SuccessResponse(result, "Login successful"));
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning("Failed login attempt for email: {Email}", request.Email);
            return Unauthorized(ApiResponse<object>.ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// Validate JWT token
    /// </summary>
    /// <param name="token">JWT token to validate</param>
    /// <returns>Boolean indicating token validity</returns>
    [HttpPost("validate-token")]
    [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
    public async Task<ActionResult<ApiResponse<bool>>> ValidateToken([FromBody] string token)
    {
        var isValid = await _authService.ValidateTokenAsync(token);
        return Ok(ApiResponse<bool>.SuccessResponse(isValid));
    }
}
