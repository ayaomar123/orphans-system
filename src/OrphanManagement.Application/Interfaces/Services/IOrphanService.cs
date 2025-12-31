using OrphanManagement.Application.DTOs.Common;
using OrphanManagement.Application.DTOs.Orphans;
using OrphanManagement.Domain.Enums;

namespace OrphanManagement.Application.Interfaces.Services;

/// <summary>
/// Service interface for orphan management operations
/// </summary>
public interface IOrphanService
{
    /// <summary>
    /// Get orphans with pagination and filters
    /// </summary>
    Task<PagedResult<OrphanDto>> GetOrphansAsync(
        int pageNumber,
        int pageSize,
        string? searchTerm = null,
        string? city = null,
        Gender? gender = null,
        SponsorshipStatus? sponsorshipStatus = null,
        EducationStatus? educationStatus = null,
        int? minAge = null,
        int? maxAge = null,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get orphan by ID with full details
    /// </summary>
    Task<OrphanDto?> GetOrphanByIdAsync(Guid id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Create a new orphan record
    /// </summary>
    Task<OrphanDto> CreateOrphanAsync(CreateOrphanRequest request, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Update an existing orphan record
    /// </summary>
    Task<OrphanDto> UpdateOrphanAsync(Guid id, UpdateOrphanRequest request, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Delete an orphan record
    /// </summary>
    Task DeleteOrphanAsync(Guid id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Upload orphan profile photo
    /// </summary>
    Task<string> UploadPhotoAsync(Guid id, Stream fileStream, string fileName, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get orphan statistics by city
    /// </summary>
    Task<Dictionary<string, int>> GetOrphanStatsByCityAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get orphan statistics by age group
    /// </summary>
    Task<Dictionary<string, int>> GetOrphanStatsByAgeGroupAsync(CancellationToken cancellationToken = default);
}
