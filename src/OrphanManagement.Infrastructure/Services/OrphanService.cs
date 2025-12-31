using AutoMapper;
using Microsoft.Extensions.Logging;
using OrphanManagement.Application.DTOs.Common;
using OrphanManagement.Application.DTOs.Orphans;
using OrphanManagement.Application.Interfaces.Repositories;
using OrphanManagement.Application.Interfaces.Services;
using OrphanManagement.Domain.Entities;
using OrphanManagement.Domain.Enums;
using OrphanManagement.Domain.Exceptions;

namespace OrphanManagement.Infrastructure.Services;

public class OrphanService : IOrphanService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<OrphanService> _logger;

    public OrphanService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<OrphanService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<PagedResult<OrphanDto>> GetOrphansAsync(
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
        _logger.LogInformation("Getting orphans with pagination. Page: {PageNumber}, Size: {PageSize}", pageNumber, pageSize);

        var pagedOrphans = await _unitOfWork.Orphans.GetPagedAsync(
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

        var orphanDtos = _mapper.Map<List<OrphanDto>>(pagedOrphans.Items);

        return new PagedResult<OrphanDto>
        {
            Items = orphanDtos,
            TotalCount = pagedOrphans.TotalCount,
            PageNumber = pagedOrphans.PageNumber,
            PageSize = pagedOrphans.PageSize,
            TotalPages = pagedOrphans.TotalPages
        };
    }

    public async Task<OrphanDto?> GetOrphanByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting orphan by ID: {OrphanId}", id);

        var orphan = await _unitOfWork.Orphans.GetByIdWithDetailsAsync(id, cancellationToken);
        
        if (orphan == null)
        {
            _logger.LogWarning("Orphan not found with ID: {OrphanId}", id);
            return null;
        }

        return _mapper.Map<OrphanDto>(orphan);
    }

    public async Task<OrphanDto> CreateOrphanAsync(CreateOrphanRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating new orphan: {FirstName} {LastName}", request.FirstName, request.LastName);

        if (!string.IsNullOrEmpty(request.NationalId))
        {
            var existingOrphan = await _unitOfWork.Orphans.FirstOrDefaultAsync(
                o => o.NationalId == request.NationalId,
                cancellationToken);

            if (existingOrphan != null)
            {
                throw new DomainException($"An orphan with National ID '{request.NationalId}' already exists.");
            }
        }

        var orphan = _mapper.Map<Orphan>(request);
        orphan.CreatedAt = DateTime.UtcNow;
        orphan.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.Orphans.AddAsync(orphan, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Orphan created successfully with ID: {OrphanId}", orphan.Id);

        return _mapper.Map<OrphanDto>(orphan);
    }

    public async Task<OrphanDto> UpdateOrphanAsync(Guid id, UpdateOrphanRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating orphan with ID: {OrphanId}", id);

        var orphan = await _unitOfWork.Orphans.GetByIdAsync(id, cancellationToken);
        
        if (orphan == null)
        {
            throw new NotFoundException("Orphan", id);
        }

        if (!string.IsNullOrEmpty(request.NationalId) && request.NationalId != orphan.NationalId)
        {
            var existingOrphan = await _unitOfWork.Orphans.FirstOrDefaultAsync(
                o => o.NationalId == request.NationalId && o.Id != id,
                cancellationToken);

            if (existingOrphan != null)
            {
                throw new DomainException($"An orphan with National ID '{request.NationalId}' already exists.");
            }
        }

        _mapper.Map(request, orphan);
        orphan.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Orphans.Update(orphan);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Orphan updated successfully with ID: {OrphanId}", id);

        return _mapper.Map<OrphanDto>(orphan);
    }

    public async Task DeleteOrphanAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting orphan with ID: {OrphanId}", id);

        var orphan = await _unitOfWork.Orphans.GetByIdAsync(id, cancellationToken);
        
        if (orphan == null)
        {
            throw new NotFoundException("Orphan", id);
        }

        _unitOfWork.Orphans.Delete(orphan);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Orphan deleted successfully with ID: {OrphanId}", id);
    }

    public async Task<string> UploadPhotoAsync(Guid id, Stream fileStream, string fileName, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Uploading photo for orphan with ID: {OrphanId}", id);

        var orphan = await _unitOfWork.Orphans.GetByIdAsync(id, cancellationToken);
        
        if (orphan == null)
        {
            throw new NotFoundException("Orphan", id);
        }

        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "uploads", "orphans");
        
        if (!Directory.Exists(uploadsFolder))
        {
            Directory.CreateDirectory(uploadsFolder);
        }

        var uniqueFileName = $"{id}_{Guid.NewGuid()}{Path.GetExtension(fileName)}";
        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        using (var fileStreamOutput = new FileStream(filePath, FileMode.Create))
        {
            await fileStream.CopyToAsync(fileStreamOutput, cancellationToken);
        }

        var photoUrl = $"/uploads/orphans/{uniqueFileName}";
        orphan.PhotoUrl = photoUrl;
        orphan.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Orphans.Update(orphan);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Photo uploaded successfully for orphan with ID: {OrphanId}", id);

        return photoUrl;
    }

    public async Task<Dictionary<string, int>> GetOrphanStatsByCityAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting orphan statistics by city");

        var stats = await _unitOfWork.Orphans.GetOrphanCountByCityAsync(cancellationToken);

        return stats;
    }

    public async Task<Dictionary<string, int>> GetOrphanStatsByAgeGroupAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting orphan statistics by age group");

        var stats = await _unitOfWork.Orphans.GetOrphanCountByAgeGroupAsync(cancellationToken);

        return stats;
    }
}
