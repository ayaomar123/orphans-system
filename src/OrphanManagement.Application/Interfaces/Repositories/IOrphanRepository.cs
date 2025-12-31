using OrphanManagement.Application.DTOs.Common;
using OrphanManagement.Domain.Entities;
using OrphanManagement.Domain.Enums;

namespace OrphanManagement.Application.Interfaces.Repositories;

/// <summary>
/// Extended repository interface for Orphan entity with custom query methods
/// </summary>
public interface IOrphanRepository : IRepository<Orphan>
{
    /// <summary>
    /// Get paginated and filtered list of orphans
    /// </summary>
    Task<PagedResult<Orphan>> GetPagedAsync(
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
    /// Get orphan with all related data (sponsorships, events)
    /// </summary>
    Task<Orphan?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get orphans by city
    /// </summary>
    Task<List<Orphan>> GetByCityAsync(string city, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get statistics grouped by city
    /// </summary>
    Task<Dictionary<string, int>> GetOrphanCountByCityAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get statistics grouped by age range
    /// </summary>
    Task<Dictionary<string, int>> GetOrphanCountByAgeGroupAsync(CancellationToken cancellationToken = default);
}
