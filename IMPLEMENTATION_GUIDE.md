# Implementation Guide - Orphan Management System

This guide provides complete code examples for implementing the full-stack Orphan Management System.

## Table of Contents
1. [Backend Implementation](#backend-implementation)
2. [Frontend Implementation](#frontend-implementation)
3. [Running the Application](#running-the-application)

---

## Backend Implementation

### 1. Complete OrphanRepository Implementation

**File**: `src/OrphanManagement.Infrastructure/Repositories/OrphanRepository.cs`

```csharp
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

        // Apply filters
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

        // Age filtering (requires calculation)
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

        // Get total count before pagination
        var totalCount = await query.CountAsync(cancellationToken);

        // Apply pagination
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
```

### 2. Complete AuthService Implementation

**File**: `src/OrphanManagement.Infrastructure/Services/AuthService.cs`

```csharp
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using OrphanManagement.Application.DTOs.Auth;
using OrphanManagement.Application.Interfaces.Repositories;
using OrphanManagement.Application.Interfaces.Services;
using OrphanManagement.Domain.Exceptions;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace OrphanManagement.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IConfiguration _configuration;

    public AuthService(IUnitOfWork unitOfWork, IConfiguration configuration)
    {
        _unitOfWork = unitOfWork;
        _configuration = configuration;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        // Find user by email
        var user = await _unitOfWork.Users.FirstOrDefaultAsync(
            u => u.Email == request.Email,
            cancellationToken);

        if (user == null || !user.IsActive)
        {
            throw new UnauthorizedAccessException("Invalid email or password");
        }

        // Verify password
        if (!VerifyPassword(request.Password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Invalid email or password");
        }

        // Update last login
        user.LastLoginAt = DateTime.UtcNow;
        _unitOfWork.Users.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Generate JWT token
        var token = GenerateJwtToken(user);
        var expirationMinutes = int.Parse(_configuration["JwtSettings:ExpirationInMinutes"] ?? "60");

        return new LoginResponse
        {
            Token = token,
            ExpiresAt = DateTime.UtcNow.AddMinutes(expirationMinutes),
            UserId = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            Role = user.Role.ToString()
        };
    }

    public Task<bool> ValidateTokenAsync(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var secret = _configuration["JwtSettings:Secret"];
            
            if (string.IsNullOrEmpty(secret))
            {
                return Task.FromResult(false);
            }

            var key = Encoding.ASCII.GetBytes(secret);

            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _configuration["JwtSettings:Issuer"],
                ValidateAudience = true,
                ValidAudience = _configuration["JwtSettings:Audience"],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            }, out _);

            return Task.FromResult(true);
        }
        catch
        {
            return Task.FromResult(false);
        }
    }

    public string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password, 12);
    }

    public bool VerifyPassword(string password, string passwordHash)
    {
        return BCrypt.Net.BCrypt.Verify(password, passwordHash);
    }

    private string GenerateJwtToken(Domain.Entities.User user)
    {
        var secret = _configuration["JwtSettings:Secret"];
        var issuer = _configuration["JwtSettings:Issuer"];
        var audience = _configuration["JwtSettings:Audience"];
        var expirationMinutes = int.Parse(_configuration["JwtSettings:ExpirationInMinutes"] ?? "60");

        if (string.IsNullOrEmpty(secret))
        {
            throw new InvalidOperationException("JWT Secret is not configured");
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Name, user.FullName),
            new Claim(ClaimTypes.Role, user.Role.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
```

### 3. Complete Program.cs with Dependency Injection

**File**: `src/OrphanManagement.API/Program.cs`

```csharp
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OrphanManagement.Application.Interfaces.Repositories;
using OrphanManagement.Application.Interfaces.Services;
using OrphanManagement.Application.Mappings;
using OrphanManagement.Infrastructure.Data;
using OrphanManagement.Infrastructure.Services;
using OrphanManagement.Infrastructure.Repositories;
using Serilog;
using System.Text;
using FluentValidation.AspNetCore;
using FluentValidation;
using OrphanManagement.API.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .WriteTo.Console()
    .WriteTo.File("logs/orphan-management-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container
builder.Services.AddControllers();

// Add FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<OrphanManagement.Application.Validators.CreateUserRequestValidator>();

// Configure Database
var usePostgreSQL = builder.Configuration.GetValue<bool>("UsePostgreSQL");
if (usePostgreSQL)
{
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSQL")));
}
else
{
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
}

// Configure AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// Register Repositories
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IOrphanRepository, OrphanRepository>();
builder.Services.AddScoped<IEventRepository, EventRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Register Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IOrphanService, OrphanService>();
builder.Services.AddScoped<IEventService, EventService>();

// Configure JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secret = jwtSettings["Secret"];

if (string.IsNullOrEmpty(secret))
{
    throw new InvalidOperationException("JWT Secret is not configured");
}

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// Configure CORS
var corsOrigins = builder.Configuration.GetSection("CorsOrigins").Get<string[]>() ?? new[] { "http://localhost:4200" };
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", policy =>
    {
        policy.WithOrigins(corsOrigins)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// Configure Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Orphan Management API",
        Version = "v1",
        Description = "API for managing orphan information, events, and sponsorships",
        Contact = new OpenApiContact
        {
            Name = "Development Team",
            Email = "dev@orphanmanagement.com"
        }
    });

    // Add JWT Authentication to Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Orphan Management API V1");
        c.RoutePrefix = "swagger";
    });
}

// Global exception handling middleware
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();
app.UseStaticFiles(); // For serving uploaded files

app.UseCors("AllowAngularApp");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Seed database with default admin user
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        var authService = services.GetRequiredService<IAuthService>();
        
        // Apply migrations
        await context.Database.MigrateAsync();
        
        // Seed default admin user if not exists
        await SeedDefaultUser(context, authService);
    }
    catch (Exception ex)
    {
        Log.Error(ex, "An error occurred while migrating or seeding the database");
    }
}

app.Run();

// Helper method to seed default admin user
static async Task SeedDefaultUser(ApplicationDbContext context, IAuthService authService)
{
    if (!await context.Users.AnyAsync())
    {
        var adminUser = new OrphanManagement.Domain.Entities.User
        {
            Id = Guid.NewGuid(),
            FullName = "System Administrator",
            Email = "admin@orphan.com",
            PasswordHash = authService.HashPassword("Admin@123"),
            Role = OrphanManagement.Domain.Enums.UserRole.Admin,
            Phone = "+1234567890",
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        context.Users.Add(adminUser);
        await context.SaveChangesAsync();
        
        Log.Information("Default admin user created: admin@orphan.com / Admin@123");
    }
}
```

### 4. Example Controller - OrphansController

**File**: `src/OrphanManagement.API/Controllers/OrphansController.cs`

```csharp
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrphanManagement.Application.DTOs.Common;
using OrphanManagement.Application.DTOs.Orphans;
using OrphanManagement.Application.Interfaces.Services;
using OrphanManagement.Domain.Enums;

namespace OrphanManagement.API.Controllers;

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
    /// Get paginated list of orphans with optional filters
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<OrphanDto>>>> GetOrphans(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? searchTerm = null,
        [FromQuery] string? city = null,
        [FromQuery] Gender? gender = null,
        [FromQuery] SponsorshipStatus? sponsorshipStatus = null,
        [FromQuery] EducationStatus? educationStatus = null,
        [FromQuery] int? minAge = null,
        [FromQuery] int? maxAge = null)
    {
        _logger.LogInformation("Getting orphans list - Page: {PageNumber}, Size: {PageSize}", pageNumber, pageSize);

        var result = await _orphanService.GetOrphansAsync(
            pageNumber, pageSize, searchTerm, city, gender, 
            sponsorshipStatus, educationStatus, minAge, maxAge);

        return Ok(ApiResponse<PagedResult<OrphanDto>>.SuccessResponse(result));
    }

    /// <summary>
    /// Get orphan by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<OrphanDto>>> GetOrphanById(Guid id)
    {
        _logger.LogInformation("Getting orphan with ID: {OrphanId}", id);

        var orphan = await _orphanService.GetOrphanByIdAsync(id);
        
        if (orphan == null)
        {
            return NotFound(ApiResponse<OrphanDto>.ErrorResponse("Orphan not found"));
        }

        return Ok(ApiResponse<OrphanDto>.SuccessResponse(orphan));
    }

    /// <summary>
    /// Create a new orphan
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin,SocialWorker")]
    public async Task<ActionResult<ApiResponse<OrphanDto>>> CreateOrphan([FromBody] CreateOrphanRequest request)
    {
        _logger.LogInformation("Creating new orphan: {FirstName} {LastName}", request.FirstName, request.LastName);

        var orphan = await _orphanService.CreateOrphanAsync(request);

        return CreatedAtAction(
            nameof(GetOrphanById),
            new { id = orphan.Id },
            ApiResponse<OrphanDto>.SuccessResponse(orphan, "Orphan created successfully"));
    }

    /// <summary>
    /// Update an existing orphan
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,SocialWorker")]
    public async Task<ActionResult<ApiResponse<OrphanDto>>> UpdateOrphan(Guid id, [FromBody] UpdateOrphanRequest request)
    {
        _logger.LogInformation("Updating orphan with ID: {OrphanId}", id);

        var orphan = await _orphanService.UpdateOrphanAsync(id, request);

        return Ok(ApiResponse<OrphanDto>.SuccessResponse(orphan, "Orphan updated successfully"));
    }

    /// <summary>
    /// Delete an orphan
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteOrphan(Guid id)
    {
        _logger.LogInformation("Deleting orphan with ID: {OrphanId}", id);

        await _orphanService.DeleteOrphanAsync(id);

        return Ok(ApiResponse<object>.SuccessResponse(null, "Orphan deleted successfully"));
    }

    /// <summary>
    /// Upload orphan photo
    /// </summary>
    [HttpPost("{id}/photo")]
    [Authorize(Roles = "Admin,SocialWorker")]
    public async Task<ActionResult<ApiResponse<string>>> UploadPhoto(Guid id, IFormFile file)
    {
        _logger.LogInformation("Uploading photo for orphan: {OrphanId}", id);

        if (file == null || file.Length == 0)
        {
            return BadRequest(ApiResponse<string>.ErrorResponse("No file provided"));
        }

        using var stream = file.OpenReadStream();
        var photoUrl = await _orphanService.UploadPhotoAsync(id, stream, file.FileName);

        return Ok(ApiResponse<string>.SuccessResponse(photoUrl, "Photo uploaded successfully"));
    }

    /// <summary>
    /// Get orphan statistics by city
    /// </summary>
    [HttpGet("statistics/by-city")]
    public async Task<ActionResult<ApiResponse<Dictionary<string, int>>>> GetStatsByCity()
    {
        var stats = await _orphanService.GetOrphanStatsByCityAsync();
        return Ok(ApiResponse<Dictionary<string, int>>.SuccessResponse(stats));
    }

    /// <summary>
    /// Get orphan statistics by age group
    /// </summary>
    [HttpGet("statistics/by-age-group")]
    public async Task<ActionResult<ApiResponse<Dictionary<string, int>>>> GetStatsByAgeGroup()
    {
        var stats = await _orphanService.GetOrphanStatsByAgeGroupAsync();
        return Ok(ApiResponse<Dictionary<string, int>>.SuccessResponse(stats));
    }
}
```

### 5. Global Exception Handling Middleware

**File**: `src/OrphanManagement.API/Middleware/ExceptionHandlingMiddleware.cs`

```csharp
using OrphanManagement.Application.DTOs.Common;
using OrphanManagement.Domain.Exceptions;
using System.Net;
using System.Text.Json;

namespace OrphanManagement.API.Middleware;

/// <summary>
/// Global exception handling middleware to catch and format all exceptions
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var response = exception switch
        {
            NotFoundException => (HttpStatusCode.NotFound, 
                ApiResponse<object>.ErrorResponse(exception.Message)),
            
            UnauthorizedAccessException => (HttpStatusCode.Unauthorized, 
                ApiResponse<object>.ErrorResponse(exception.Message)),
            
            FluentValidation.ValidationException validationEx => (HttpStatusCode.BadRequest,
                ApiResponse<object>.ErrorResponse("Validation failed", 
                    validationEx.Errors.Select(e => e.ErrorMessage).ToList())),
            
            DomainException => (HttpStatusCode.BadRequest, 
                ApiResponse<object>.ErrorResponse(exception.Message)),
            
            _ => (HttpStatusCode.InternalServerError, 
                ApiResponse<object>.ErrorResponse("An internal server error occurred"))
        };

        context.Response.StatusCode = (int)response.Item1;

        var json = JsonSerializer.Serialize(response.Item2, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(json);
    }
}
```

---

## Frontend Implementation (Angular 17+)

### 1. Create Angular Project

```bash
cd /home/engine/project
ng new orphan-management-ui --standalone --routing --style=scss
cd orphan-management-ui
```

### 2. Install Dependencies

```bash
npm install @angular/material @angular/cdk
npm install @ngx-translate/core @ngx-translate/http-loader
npm install chart.js ng2-charts
npm install --save-dev @types/chart.js
```

### 3. Environment Configuration

**File**: `orphan-management-ui/src/environments/environment.ts`

```typescript
export const environment = {
  production: false,
  apiUrl: 'https://localhost:7001/api',
  tokenKey: 'auth_token',
  userKey: 'current_user'
};
```

### 4. Auth Service

**File**: `orphan-management-ui/src/app/core/services/auth.service.ts`

```typescript
import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { Router } from '@angular/router';
import { environment } from '../../../environments/environment';

export interface LoginRequest {
  email: string;
  password: string;
}

export interface LoginResponse {
  token: string;
  expiresAt: string;
  userId: string;
  fullName: string;
  email: string;
  role: string;
}

export interface ApiResponse<T> {
  success: boolean;
  message: string;
  data?: T;
  errors?: string[];
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private http = inject(HttpClient);
  private router = inject(Router);

  private currentUserSubject = new BehaviorSubject<LoginResponse | null>(
    this.getUserFromStorage()
  );
  public currentUser$ = this.currentUserSubject.asObservable();

  get isAuthenticated(): boolean {
    return !!this.getToken();
  }

  get currentUser(): LoginResponse | null {
    return this.currentUserSubject.value;
  }

  get userRole(): string | null {
    return this.currentUser?.role || null;
  }

  login(credentials: LoginRequest): Observable<ApiResponse<LoginResponse>> {
    return this.http.post<ApiResponse<LoginResponse>>(
      `${environment.apiUrl}/auth/login`,
      credentials
    ).pipe(
      tap(response => {
        if (response.success && response.data) {
          this.setToken(response.data.token);
          this.setUser(response.data);
          this.currentUserSubject.next(response.data);
        }
      })
    );
  }

  logout(): void {
    localStorage.removeItem(environment.tokenKey);
    localStorage.removeItem(environment.userKey);
    this.currentUserSubject.next(null);
    this.router.navigate(['/login']);
  }

  getToken(): string | null {
    return localStorage.getItem(environment.tokenKey);
  }

  hasRole(roles: string[]): boolean {
    const userRole = this.userRole;
    return userRole ? roles.includes(userRole) : false;
  }

  private setToken(token: string): void {
    localStorage.setItem(environment.tokenKey, token);
  }

  private setUser(user: LoginResponse): void {
    localStorage.setItem(environment.userKey, JSON.stringify(user));
  }

  private getUserFromStorage(): LoginResponse | null {
    const userJson = localStorage.getItem(environment.userKey);
    return userJson ? JSON.parse(userJson) : null;
  }
}
```

### 5. Auth Interceptor

**File**: `orphan-management-ui/src/app/core/interceptors/auth.interceptor.ts`

```typescript
import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { AuthService } from '../services/auth.service';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);
  const token = authService.getToken();

  if (token) {
    const clonedRequest = req.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`
      }
    });
    return next(clonedRequest);
  }

  return next(req);
};
```

### 6. Error Interceptor

**File**: `orphan-management-ui/src/app/core/interceptors/error.interceptor.ts`

```typescript
import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, throwError } from 'rxjs';
import { Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const router = inject(Router);
  const authService = inject(AuthService);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      if (error.status === 401) {
        // Unauthorized - redirect to login
        authService.logout();
      } else if (error.status === 403) {
        // Forbidden - show error or redirect
        console.error('Access forbidden');
        router.navigate(['/dashboard']);
      }

      return throwError(() => error);
    })
  );
};
```

### 7. Auth Guard

**File**: `orphan-management-ui/src/app/core/guards/auth.guard.ts`

```typescript
import { inject } from '@angular/core';
import { Router, CanActivateFn } from '@angular/router';
import { AuthService } from '../services/auth.service';

export const authGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  if (authService.isAuthenticated) {
    // Check for required roles if specified
    const requiredRoles = route.data['roles'] as string[];
    
    if (requiredRoles && requiredRoles.length > 0) {
      if (authService.hasRole(requiredRoles)) {
        return true;
      } else {
        router.navigate(['/dashboard']);
        return false;
      }
    }
    
    return true;
  }

  router.navigate(['/login'], { queryParams: { returnUrl: state.url } });
  return false;
};
```

### 8. Login Component

**File**: `orphan-management-ui/src/app/features/auth/login/login.component.ts`

```typescript
import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit {
  private fb = inject(FormBuilder);
  private authService = inject(AuthService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);

  loginForm!: FormGroup;
  loading = false;
  error = '';
  returnUrl = '/dashboard';

  ngOnInit(): void {
    this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', Validators.required]
    });

    // Get return URL from route parameters or default to dashboard
    this.returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/dashboard';

    // Redirect if already logged in
    if (this.authService.isAuthenticated) {
      this.router.navigate([this.returnUrl]);
    }
  }

  onSubmit(): void {
    if (this.loginForm.invalid) {
      return;
    }

    this.loading = true;
    this.error = '';

    this.authService.login(this.loginForm.value).subscribe({
      next: (response) => {
        if (response.success) {
          this.router.navigate([this.returnUrl]);
        } else {
          this.error = response.message;
          this.loading = false;
        }
      },
      error: (error) => {
        this.error = error.error?.message || 'Login failed. Please try again.';
        this.loading = false;
      }
    });
  }

  get email() {
    return this.loginForm.get('email');
  }

  get password() {
    return this.loginForm.get('password');
  }
}
```

**File**: `orphan-management-ui/src/app/features/auth/login/login.component.html`

```html
<div class="login-container">
  <div class="login-card">
    <div class="login-header">
      <h1>Orphan Management System</h1>
      <p>Sign in to your account</p>
    </div>

    <form [formGroup]="loginForm" (ngSubmit)="onSubmit()">
      <div class="form-group">
        <label for="email">Email</label>
        <input
          type="email"
          id="email"
          formControlName="email"
          [class.invalid]="email?.invalid && email?.touched"
          placeholder="Enter your email"
        />
        <div class="error-message" *ngIf="email?.invalid && email?.touched">
          <span *ngIf="email?.errors?.['required']">Email is required</span>
          <span *ngIf="email?.errors?.['email']">Invalid email format</span>
        </div>
      </div>

      <div class="form-group">
        <label for="password">Password</label>
        <input
          type="password"
          id="password"
          formControlName="password"
          [class.invalid]="password?.invalid && password?.touched"
          placeholder="Enter your password"
        />
        <div class="error-message" *ngIf="password?.invalid && password?.touched">
          <span *ngIf="password?.errors?.['required']">Password is required</span>
        </div>
      </div>

      <div class="error-message" *ngIf="error">
        {{ error }}
      </div>

      <button type="submit" [disabled]="loading || loginForm.invalid" class="btn-primary">
        <span *ngIf="!loading">Sign In</span>
        <span *ngIf="loading">Signing in...</span>
      </button>
    </form>

    <div class="login-footer">
      <p>Default credentials: admin@orphan.com / Admin@123</p>
    </div>
  </div>
</div>
```

### 9. Orphans Service

**File**: `orphan-management-ui/src/app/features/orphans/orphans.service.ts`

```typescript
import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse } from '../../core/services/auth.service';

export interface OrphanDto {
  id: string;
  firstName: string;
  lastName: string;
  fullName: string;
  gender: string;
  dateOfBirth: string;
  age: number;
  nationalId?: string;
  city: string;
  address?: string;
  healthStatus: string;
  healthNotes?: string;
  educationStatus: string;
  schoolName?: string;
  classLevel?: string;
  guardianName?: string;
  guardianPhone?: string;
  guardianRelationship?: string;
  sponsorshipStatus: string;
  photoUrl?: string;
  notes?: string;
  createdAt: string;
  updatedAt: string;
}

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}

export interface OrphanFilters {
  pageNumber?: number;
  pageSize?: number;
  searchTerm?: string;
  city?: string;
  gender?: string;
  sponsorshipStatus?: string;
  educationStatus?: string;
  minAge?: number;
  maxAge?: number;
}

@Injectable({
  providedIn: 'root'
})
export class OrphansService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/orphans`;

  getOrphans(filters: OrphanFilters): Observable<ApiResponse<PagedResult<OrphanDto>>> {
    let params = new HttpParams();

    Object.keys(filters).forEach(key => {
      const value = (filters as any)[key];
      if (value !== null && value !== undefined && value !== '') {
        params = params.append(key, value.toString());
      }
    });

    return this.http.get<ApiResponse<PagedResult<OrphanDto>>>(this.apiUrl, { params });
  }

  getOrphanById(id: string): Observable<ApiResponse<OrphanDto>> {
    return this.http.get<ApiResponse<OrphanDto>>(`${this.apiUrl}/${id}`);
  }

  createOrphan(orphan: Partial<OrphanDto>): Observable<ApiResponse<OrphanDto>> {
    return this.http.post<ApiResponse<OrphanDto>>(this.apiUrl, orphan);
  }

  updateOrphan(id: string, orphan: Partial<OrphanDto>): Observable<ApiResponse<OrphanDto>> {
    return this.http.put<ApiResponse<OrphanDto>>(`${this.apiUrl}/${id}`, orphan);
  }

  deleteOrphan(id: string): Observable<ApiResponse<any>> {
    return this.http.delete<ApiResponse<any>>(`${this.apiUrl}/${id}`);
  }

  uploadPhoto(id: string, file: File): Observable<ApiResponse<string>> {
    const formData = new FormData();
    formData.append('file', file);
    return this.http.post<ApiResponse<string>>(`${this.apiUrl}/${id}/photo`, formData);
  }

  getStatsByCity(): Observable<ApiResponse<Record<string, number>>> {
    return this.http.get<ApiResponse<Record<string, number>>>(`${this.apiUrl}/statistics/by-city`);
  }

  getStatsByAgeGroup(): Observable<ApiResponse<Record<string, number>>> {
    return this.http.get<ApiResponse<Record<string, number>>>(`${this.apiUrl}/statistics/by-age-group`);
  }
}
```

### 10. App Routes Configuration

**File**: `orphan-management-ui/src/app/app.routes.ts`

```typescript
import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';

export const routes: Routes = [
  {
    path: 'login',
    loadComponent: () => import('./features/auth/login/login.component').then(m => m.LoginComponent)
  },
  {
    path: 'dashboard',
    canActivate: [authGuard],
    loadComponent: () => import('./features/dashboard/dashboard.component').then(m => m.DashboardComponent)
  },
  {
    path: 'users',
    canActivate: [authGuard],
    data: { roles: ['Admin'] },
    loadChildren: () => import('./features/users/users.routes').then(m => m.USERS_ROUTES)
  },
  {
    path: 'orphans',
    canActivate: [authGuard],
    loadChildren: () => import('./features/orphans/orphans.routes').then(m => m.ORPHANS_ROUTES)
  },
  {
    path: 'events',
    canActivate: [authGuard],
    loadChildren: () => import('./features/events/events.routes').then(m => m.EVENTS_ROUTES)
  },
  {
    path: '',
    redirectTo: '/dashboard',
    pathMatch: 'full'
  },
  {
    path: '**',
    redirectTo: '/dashboard'
  }
];
```

### 11. App Configuration

**File**: `orphan-management-ui/src/app/app.config.ts`

```typescript
import { ApplicationConfig, importProvidersFrom } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { provideAnimations } from '@angular/platform-browser/animations';
import { TranslateModule, TranslateLoader } from '@ngx-translate/core';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';
import { HttpClient } from '@angular/common/http';

import { routes } from './app.routes';
import { authInterceptor } from './core/interceptors/auth.interceptor';
import { errorInterceptor } from './core/interceptors/error.interceptor';

export function HttpLoaderFactory(http: HttpClient) {
  return new TranslateHttpLoader(http, './assets/i18n/', '.json');
}

export const appConfig: ApplicationConfig = {
  providers: [
    provideRouter(routes),
    provideHttpClient(
      withInterceptors([authInterceptor, errorInterceptor])
    ),
    provideAnimations(),
    importProvidersFrom(
      TranslateModule.forRoot({
        defaultLanguage: 'en',
        loader: {
          provide: TranslateLoader,
          useFactory: HttpLoaderFactory,
          deps: [HttpClient]
        }
      })
    )
  ]
};
```

---

## Running the Application

### Backend

1. **Update Connection String** in `appsettings.json`
2. **Run Migrations**:
   ```bash
   cd src/OrphanManagement.API
   dotnet ef migrations add InitialCreate --project ../OrphanManagement.Infrastructure
   dotnet ef database update
   ```
3. **Run API**:
   ```bash
   dotnet run
   ```
   API will be available at `https://localhost:7001`

### Frontend

1. **Install Dependencies**:
   ```bash
   cd orphan-management-ui
   npm install
   ```
2. **Run Development Server**:
   ```bash
   ng serve
   ```
   App will be available at `http://localhost:4200`

### Default Login
- **Email**: admin@orphan.com
- **Password**: Admin@123

---

## Next Steps

1. Complete remaining service implementations (UserService, OrphanService, EventService)
2. Implement remaining repository classes (EventRepository, UnitOfWork)
3. Create remaining Angular components (orphan-list, orphan-form, event-list, etc.)
4. Add unit tests for critical functionality
5. Configure production builds and deployment

This implementation guide provides the foundation for a complete, production-ready Orphan Management System following Clean Architecture principles and modern best practices.
