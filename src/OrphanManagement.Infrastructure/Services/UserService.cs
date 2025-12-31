using AutoMapper;
using Microsoft.Extensions.Logging;
using OrphanManagement.Application.DTOs.Common;
using OrphanManagement.Application.DTOs.Users;
using OrphanManagement.Application.Interfaces.Repositories;
using OrphanManagement.Application.Interfaces.Services;
using OrphanManagement.Domain.Entities;
using OrphanManagement.Domain.Enums;
using OrphanManagement.Domain.Exceptions;

namespace OrphanManagement.Infrastructure.Services;

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IAuthService _authService;
    private readonly ILogger<UserService> _logger;

    public UserService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IAuthService authService,
        ILogger<UserService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _authService = authService;
        _logger = logger;
    }

    public async Task<PagedResult<UserDto>> GetUsersAsync(
        int pageNumber,
        int pageSize,
        string? searchTerm = null,
        string? role = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting users with pagination. Page: {PageNumber}, Size: {PageSize}", pageNumber, pageSize);

        var allUsers = await _unitOfWork.Users.GetAllAsync(cancellationToken);

        var filteredUsers = allUsers.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var search = searchTerm.ToLower();
            filteredUsers = filteredUsers.Where(u => 
                u.FullName.ToLower().Contains(search) || 
                u.Email.ToLower().Contains(search));
        }

        if (!string.IsNullOrWhiteSpace(role) && Enum.TryParse<UserRole>(role, true, out var userRole))
        {
            filteredUsers = filteredUsers.Where(u => u.Role == userRole);
        }

        var totalCount = filteredUsers.Count();

        var users = filteredUsers
            .OrderBy(u => u.FullName)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var userDtos = _mapper.Map<List<UserDto>>(users);

        return new PagedResult<UserDto>
        {
            Items = userDtos,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        };
    }

    public async Task<UserDto?> GetUserByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting user by ID: {UserId}", id);

        var user = await _unitOfWork.Users.GetByIdAsync(id, cancellationToken);
        
        if (user == null)
        {
            _logger.LogWarning("User not found with ID: {UserId}", id);
            return null;
        }

        return _mapper.Map<UserDto>(user);
    }

    public async Task<UserDto?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting user by email: {Email}", email);

        var user = await _unitOfWork.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
        
        if (user == null)
        {
            _logger.LogWarning("User not found with email: {Email}", email);
            return null;
        }

        return _mapper.Map<UserDto>(user);
    }

    public async Task<UserDto> CreateUserAsync(CreateUserRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating new user: {Email}", request.Email);

        var existingUser = await _unitOfWork.Users.FirstOrDefaultAsync(
            u => u.Email.ToLower() == request.Email.ToLower(),
            cancellationToken);

        if (existingUser != null)
        {
            throw new DomainException($"A user with email '{request.Email}' already exists.");
        }

        var user = _mapper.Map<User>(request);
        user.PasswordHash = _authService.HashPassword(request.Password);
        user.CreatedAt = DateTime.UtcNow;
        user.UpdatedAt = DateTime.UtcNow;
        user.IsActive = true;

        await _unitOfWork.Users.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("User created successfully with ID: {UserId}", user.Id);

        return _mapper.Map<UserDto>(user);
    }

    public async Task<UserDto> UpdateUserAsync(Guid id, UpdateUserRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating user with ID: {UserId}", id);

        var user = await _unitOfWork.Users.GetByIdAsync(id, cancellationToken);
        
        if (user == null)
        {
            throw new NotFoundException("User", id);
        }

        if (request.Email.ToLower() != user.Email.ToLower())
        {
            var existingUser = await _unitOfWork.Users.FirstOrDefaultAsync(
                u => u.Email.ToLower() == request.Email.ToLower() && u.Id != id,
                cancellationToken);

            if (existingUser != null)
            {
                throw new DomainException($"A user with email '{request.Email}' already exists.");
            }
        }

        _mapper.Map(request, user);
        user.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Users.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("User updated successfully with ID: {UserId}", id);

        return _mapper.Map<UserDto>(user);
    }

    public async Task DeleteUserAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting user with ID: {UserId}", id);

        var user = await _unitOfWork.Users.GetByIdAsync(id, cancellationToken);
        
        if (user == null)
        {
            throw new NotFoundException("User", id);
        }

        _unitOfWork.Users.Delete(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("User deleted successfully with ID: {UserId}", id);
    }

    public async Task ToggleUserStatusAsync(Guid id, bool isActive, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Toggling user status for ID: {UserId} to {IsActive}", id, isActive);

        var user = await _unitOfWork.Users.GetByIdAsync(id, cancellationToken);
        
        if (user == null)
        {
            throw new NotFoundException("User", id);
        }

        user.IsActive = isActive;
        user.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Users.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("User status toggled successfully for ID: {UserId}", id);
    }
}
