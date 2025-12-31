using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrphanManagement.Application.DTOs.Common;
using OrphanManagement.Application.DTOs.Orphans;
using OrphanManagement.Application.Interfaces.Services;
using OrphanManagement.Domain.Enums;

namespace OrphanManagement.API.Controllers;

/// <summary>
/// Controller for orphan management operations (CRUD, filtering, statistics)
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrphansController : ControllerBase
{
    private readonly IOrphanService _orphanService;
    private readonly ILogger<OrphansController> _logger;

    public OrphansController(IOrphanService orphanService, ILogger<OrphansController> logger)
    {
        _orphanService = orphanService;
        _logger = logger;
    }

    /// <summary>
    /// Get all orphans with pagination and filtering
    /// </summary>
    /// <param name="pageNumber">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 10)</param>
    /// <param name="searchTerm">Search by name or national ID</param>
    /// <param name="city">Filter by city</param>
    /// <param name="gender">Filter by gender</param>
    /// <param name="sponsorshipStatus">Filter by sponsorship status</param>
    /// <param name="educationStatus">Filter by education status</param>
    /// <param name="minAge">Filter by minimum age</param>
    /// <param name="maxAge">Filter by maximum age</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of orphans</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<OrphanDto>>), 200)]
    public async Task<ActionResult<ApiResponse<PagedResult<OrphanDto>>>> GetOrphans(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? searchTerm = null,
        [FromQuery] string? city = null,
        [FromQuery] Gender? gender = null,
        [FromQuery] SponsorshipStatus? sponsorshipStatus = null,
        [FromQuery] EducationStatus? educationStatus = null,
        [FromQuery] int? minAge = null,
        [FromQuery] int? maxAge = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting orphans - Page: {PageNumber}, Size: {PageSize}", pageNumber, pageSize);

        var result = await _orphanService.GetOrphansAsync(
            pageNumber,
            pageSize,
            searchTerm,
            city,
            gender,
            sponsorshipStatus,
            educationStatus,
            minAge,
            maxAge,
            cancellationToken);

        return Ok(ApiResponse<PagedResult<OrphanDto>>.SuccessResponse(result));
    }

    /// <summary>
    /// Get orphan by ID
    /// </summary>
    /// <param name="id">Orphan ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Orphan details</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<OrphanDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public async Task<ActionResult<ApiResponse<OrphanDto>>> GetOrphanById(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting orphan by ID: {OrphanId}", id);

        var orphan = await _orphanService.GetOrphanByIdAsync(id, cancellationToken);

        if (orphan == null)
        {
            _logger.LogWarning("Orphan not found: {OrphanId}", id);
            return NotFound(ApiResponse<object>.ErrorResponse("Orphan not found"));
        }

        return Ok(ApiResponse<OrphanDto>.SuccessResponse(orphan));
    }

    /// <summary>
    /// Create a new orphan record
    /// </summary>
    /// <param name="request">Orphan creation data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created orphan</returns>
    [HttpPost]
    [Authorize(Roles = "Admin,SocialWorker")]
    [ProducesResponseType(typeof(ApiResponse<OrphanDto>), 201)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    public async Task<ActionResult<ApiResponse<OrphanDto>>> CreateOrphan(
        [FromBody] CreateOrphanRequest request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating new orphan: {FirstName} {LastName}", request.FirstName, request.LastName);

        try
        {
            var orphan = await _orphanService.CreateOrphanAsync(request, cancellationToken);
            
            _logger.LogInformation("Orphan created successfully: {OrphanId}", orphan.Id);
            return CreatedAtAction(
                nameof(GetOrphanById),
                new { id = orphan.Id },
                ApiResponse<OrphanDto>.SuccessResponse(orphan, "Orphan created successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating orphan");
            return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// Update an existing orphan record
    /// </summary>
    /// <param name="id">Orphan ID</param>
    /// <param name="request">Orphan update data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated orphan</returns>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,SocialWorker")]
    [ProducesResponseType(typeof(ApiResponse<OrphanDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    public async Task<ActionResult<ApiResponse<OrphanDto>>> UpdateOrphan(
        Guid id,
        [FromBody] UpdateOrphanRequest request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating orphan: {OrphanId}", id);

        try
        {
            var orphan = await _orphanService.UpdateOrphanAsync(id, request, cancellationToken);
            
            _logger.LogInformation("Orphan updated successfully: {OrphanId}", id);
            return Ok(ApiResponse<OrphanDto>.SuccessResponse(orphan, "Orphan updated successfully"));
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning("Orphan not found for update: {OrphanId}", id);
            return NotFound(ApiResponse<object>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating orphan: {OrphanId}", id);
            return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// Delete an orphan record
    /// </summary>
    /// <param name="id">Orphan ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>No content</returns>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(204)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public async Task<IActionResult> DeleteOrphan(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting orphan: {OrphanId}", id);

        try
        {
            await _orphanService.DeleteOrphanAsync(id, cancellationToken);
            
            _logger.LogInformation("Orphan deleted successfully: {OrphanId}", id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning("Orphan not found for deletion: {OrphanId}", id);
            return NotFound(ApiResponse<object>.ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// Upload orphan profile photo
    /// </summary>
    /// <param name="id">Orphan ID</param>
    /// <param name="file">Photo file</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Photo URL</returns>
    [HttpPost("{id}/photo")]
    [Authorize(Roles = "Admin,SocialWorker")]
    [ProducesResponseType(typeof(ApiResponse<string>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    public async Task<ActionResult<ApiResponse<string>>> UploadPhoto(
        Guid id,
        IFormFile file,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Uploading photo for orphan: {OrphanId}", id);

        if (file == null || file.Length == 0)
        {
            return BadRequest(ApiResponse<object>.ErrorResponse("No file uploaded"));
        }

        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        
        if (!allowedExtensions.Contains(extension))
        {
            return BadRequest(ApiResponse<object>.ErrorResponse("Invalid file type. Only images are allowed."));
        }

        try
        {
            using var stream = file.OpenReadStream();
            var photoUrl = await _orphanService.UploadPhotoAsync(id, stream, file.FileName, cancellationToken);
            
            _logger.LogInformation("Photo uploaded successfully for orphan: {OrphanId}", id);
            return Ok(ApiResponse<string>.SuccessResponse(photoUrl, "Photo uploaded successfully"));
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning("Orphan not found for photo upload: {OrphanId}", id);
            return NotFound(ApiResponse<object>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading photo for orphan: {OrphanId}", id);
            return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// Get orphan statistics by city
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Dictionary of city names and orphan counts</returns>
    [HttpGet("statistics/by-city")]
    [ProducesResponseType(typeof(ApiResponse<Dictionary<string, int>>), 200)]
    public async Task<ActionResult<ApiResponse<Dictionary<string, int>>>> GetStatsByCity(
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting orphan statistics by city");

        var stats = await _orphanService.GetOrphanStatsByCityAsync(cancellationToken);
        return Ok(ApiResponse<Dictionary<string, int>>.SuccessResponse(stats));
    }

    /// <summary>
    /// Get orphan statistics by age group
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Dictionary of age groups and orphan counts</returns>
    [HttpGet("statistics/by-age-group")]
    [ProducesResponseType(typeof(ApiResponse<Dictionary<string, int>>), 200)]
    public async Task<ActionResult<ApiResponse<Dictionary<string, int>>>> GetStatsByAgeGroup(
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting orphan statistics by age group");

        var stats = await _orphanService.GetOrphanStatsByAgeGroupAsync(cancellationToken);
        return Ok(ApiResponse<Dictionary<string, int>>.SuccessResponse(stats));
    }
}
