using Microsoft.EntityFrameworkCore;
using OrphanManagement.Application.DTOs.Common;
using OrphanManagement.Application.Interfaces.Repositories;
using OrphanManagement.Domain.Entities;
using OrphanManagement.Domain.Enums;
using OrphanManagement.Infrastructure.Data;

namespace OrphanManagement.Infrastructure.Repositories;

public class OrphanRepository : Repository<Orphan>, IOrphanRepository
{
    public OrphanRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<PagedResult<Orphan>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        string? searchTerm = null,
        string? city = null,
        Gender? gender = null,
        SponsorshipStatus? sponsorshipStatus = null,
        EducationStatus? educationStatus = null,
        int? minAge = null,
        int? maxAge = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var search = searchTerm.ToLower();
            query = query.Where(o => 
                o.FirstName.ToLower().Contains(search) ||
                o.LastName.ToLower().Contains(search) ||
                (o.NationalId != null && o.NationalId.ToLower().Contains(search)));
        }

        if (!string.IsNullOrWhiteSpace(city))
        {
            query = query.Where(o => o.City == city);
        }

        if (gender.HasValue)
        {
            query = query.Where(o => o.Gender == gender.Value);
        }

        if (sponsorshipStatus.HasValue)
        {
            query = query.Where(o => o.SponsorshipStatus == sponsorshipStatus.Value);
        }

        if (educationStatus.HasValue)
        {
            query = query.Where(o => o.EducationStatus == educationStatus.Value);
        }

        var today = DateTime.UtcNow;
        if (minAge.HasValue)
        {
            var maxBirthDate = today.AddYears(-minAge.Value);
            query = query.Where(o => o.DateOfBirth <= maxBirthDate);
        }

        if (maxAge.HasValue)
        {
            var minBirthDate = today.AddYears(-maxAge.Value - 1);
            query = query.Where(o => o.DateOfBirth >= minBirthDate);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(o => o.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Orphan>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public async Task<Orphan?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(o => o.Sponsorships)
            .Include(o => o.OrphanEvents)
                .ThenInclude(oe => oe.Event)
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
    }

    public async Task<List<Orphan>> GetByCityAsync(string city, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(o => o.City == city)
            .OrderBy(o => o.LastName)
            .ThenBy(o => o.FirstName)
            .ToListAsync(cancellationToken);
    }

    public async Task<Dictionary<string, int>> GetOrphanCountByCityAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .GroupBy(o => o.City)
            .Select(g => new { City = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.City, x => x.Count, cancellationToken);
    }

    public async Task<Dictionary<string, int>> GetOrphanCountByAgeGroupAsync(CancellationToken cancellationToken = default)
    {
        var today = DateTime.UtcNow;
        var orphans = await _dbSet.ToListAsync(cancellationToken);

        var ageGroups = orphans
            .GroupBy(o =>
            {
                var age = today.Year - o.DateOfBirth.Year;
                if (today.DayOfYear < o.DateOfBirth.DayOfYear) age--;

                return age switch
                {
                    < 6 => "0-5",
                    < 12 => "6-11",
                    < 18 => "12-17",
                    _ => "18+"
                };
            })
            .ToDictionary(g => g.Key, g => g.Count());

        return ageGroups;
    }
}
