# Orphan Management System - Architecture Documentation

## Overview

This document describes the architecture and design decisions for the Orphan Management System, a full-stack application built with .NET 8 Web API and Angular 17+.

## Clean Architecture Layers

### 1. Domain Layer (`OrphanManagement.Domain`)

**Purpose**: Contains the core business entities and domain logic. This layer has no dependencies on other layers.

**Structure**:
```
Domain/
├── Common/
│   └── BaseEntity.cs              # Base class for all entities
├── Entities/
│   ├── User.cs                    # User entity with authentication
│   ├── Orphan.cs                  # Orphan entity with profile data
│   ├── Event.cs                   # Event/activity entity
│   ├── OrphanEvent.cs             # Many-to-many join table
│   └── Sponsorship.cs             # Sponsorship tracking
├── Enums/
│   ├── UserRole.cs                # Admin, SocialWorker, Volunteer, Viewer
│   ├── Gender.cs                  # Male, Female
│   ├── SponsorshipStatus.cs       # NotSponsored, PartiallySponsored, FullySponsored
│   ├── HealthStatus.cs            # Healthy, MinorIssues, ChronicCondition, SpecialNeeds
│   ├── EducationStatus.cs         # NotEnrolled, Preschool, PrimarySchool, etc.
│   ├── EventType.cs               # Educational, Recreational, Medical, etc.
│   ├── AttendanceStatus.cs        # Registered, Attended, Absent, Cancelled
│   └── SponsorshipFrequency.cs    # OneTime, Monthly, Quarterly, Yearly
└── Exceptions/
    ├── DomainException.cs         # Base domain exception
    └── NotFoundException.cs       # Entity not found exception
```

**Key Design Decisions**:
- All entities inherit from `BaseEntity` which provides `Id`, `CreatedAt`, and `UpdatedAt`
- Computed properties in entities (e.g., `Age`, `FullName`, `IsUpcoming`) for convenience
- Enums for type safety and consistency
- No dependencies on external libraries

### 2. Application Layer (`OrphanManagement.Application`)

**Purpose**: Contains business logic, DTOs, interfaces, validators, and mappings. Depends only on the Domain layer.

**Structure**:
```
Application/
├── DTOs/
│   ├── Common/
│   │   ├── PagedResult.cs         # Generic pagination wrapper
│   │   └── ApiResponse.cs         # Standard API response wrapper
│   ├── Auth/
│   │   ├── LoginRequest.cs        # Login credentials
│   │   └── LoginResponse.cs       # JWT token response
│   ├── Users/
│   │   ├── UserDto.cs             # User data transfer object
│   │   ├── CreateUserRequest.cs   # Create user request
│   │   └── UpdateUserRequest.cs   # Update user request
│   ├── Orphans/
│   │   ├── OrphanDto.cs           # Orphan data transfer object
│   │   ├── CreateOrphanRequest.cs # Create orphan request
│   │   └── UpdateOrphanRequest.cs # Update orphan request
│   ├── Events/
│   │   ├── EventDto.cs            # Event data transfer object
│   │   ├── CreateEventRequest.cs  # Create event request
│   │   └── UpdateEventRequest.cs  # Update event request
│   └── Sponsorships/
│       └── (To be implemented)
├── Interfaces/
│   ├── Repositories/
│   │   ├── IRepository.cs         # Generic repository interface
│   │   ├── IUnitOfWork.cs         # Unit of Work pattern
│   │   ├── IOrphanRepository.cs   # Extended orphan repository
│   │   └── IEventRepository.cs    # Extended event repository
│   └── Services/
│       ├── IAuthService.cs        # Authentication service interface
│       ├── IUserService.cs        # User management service interface
│       ├── IOrphanService.cs      # Orphan management service interface
│       └── IEventService.cs       # Event management service interface
├── Services/
│   └── (To be implemented)
├── Validators/
│   ├── CreateUserRequestValidator.cs    # FluentValidation for user creation
│   ├── CreateOrphanRequestValidator.cs  # FluentValidation for orphan creation
│   └── CreateEventRequestValidator.cs   # FluentValidation for event creation
└── Mappings/
    └── MappingProfile.cs          # AutoMapper configuration
```

**Key Design Decisions**:
- DTOs separate request/response models from domain entities
- Repository pattern for data access abstraction
- Unit of Work pattern for transaction management
- Service interfaces for dependency injection
- FluentValidation for declarative validation rules
- AutoMapper for object-to-object mapping

### 3. Infrastructure Layer (`OrphanManagement.Infrastructure`)

**Purpose**: Implements interfaces from Application layer. Contains EF Core, repositories, and external service implementations.

**Structure** (To be implemented):
```
Infrastructure/
├── Data/
│   ├── ApplicationDbContext.cs    # EF Core DbContext
│   ├── Configurations/            # Entity configurations (Fluent API)
│   │   ├── UserConfiguration.cs
│   │   ├── OrphanConfiguration.cs
│   │   ├── EventConfiguration.cs
│   │   ├── OrphanEventConfiguration.cs
│   │   └── SponsorshipConfiguration.cs
│   └── Migrations/                # EF Core migrations
├── Repositories/
│   ├── Repository.cs              # Generic repository implementation
│   ├── OrphanRepository.cs        # Orphan-specific queries
│   ├── EventRepository.cs         # Event-specific queries
│   └── UnitOfWork.cs              # Unit of Work implementation
├── Services/
│   ├── AuthService.cs             # JWT generation, password hashing
│   ├── UserService.cs             # User business logic
│   ├── OrphanService.cs           # Orphan business logic
│   └── EventService.cs            # Event business logic
└── Extensions/
    └── ServiceRegistration.cs     # DI container registration
```

**Key Technologies**:
- Entity Framework Core 8.0 for ORM
- SQL Server or PostgreSQL as database
- BCrypt for password hashing
- System.IdentityModel.Tokens.Jwt for JWT tokens

### 4. API Layer (`OrphanManagement.API`)

**Purpose**: ASP.NET Core Web API layer. Contains controllers, middleware, filters, and startup configuration.

**Structure** (To be implemented):
```
API/
├── Controllers/
│   ├── AuthController.cs          # Login, register endpoints
│   ├── UsersController.cs         # User CRUD endpoints
│   ├── OrphansController.cs       # Orphan CRUD endpoints
│   ├── EventsController.cs        # Event CRUD endpoints
│   └── DashboardController.cs     # Statistics and dashboard data
├── Middleware/
│   ├── ExceptionHandlingMiddleware.cs  # Global exception handling
│   └── RequestLoggingMiddleware.cs     # Request/response logging
├── Filters/
│   └── ValidationFilter.cs        # Validation action filter
├── Extensions/
│   └── ServiceExtensions.cs       # Service registration helpers
└── Program.cs                     # Application entry point
```

**Key Features**:
- JWT Bearer authentication
- Role-based authorization attributes
- Swagger/OpenAPI documentation
- CORS configuration
- Structured logging (Serilog)
- Global exception handling

## Database Schema

### Entity Relationship Diagram (ERD)

```
┌─────────────┐
│   Users     │
├─────────────┤
│ Id (PK)     │
│ FullName    │
│ Email (UK)  │
│ PasswordHash│
│ Role        │
│ Phone       │
│ IsActive    │
│ LastLoginAt │
│ CreatedAt   │
│ UpdatedAt   │
└─────────────┘

┌──────────────┐          ┌─────────────────┐          ┌─────────────┐
│   Orphans    │          │  OrphanEvents   │          │   Events    │
├──────────────┤          ├─────────────────┤          ├─────────────┤
│ Id (PK)      │◄────────►│ OrphanId (FK)   │◄────────►│ Id (PK)     │
│ FirstName    │          │ EventId (FK)    │          │ Title       │
│ LastName     │          │ AttendanceStatus│          │ Description │
│ Gender       │          │ Notes           │          │ StartDate   │
│ DateOfBirth  │          │ RegisteredAt    │          │ EndDate     │
│ NationalId   │          └─────────────────┘          │ Location    │
│ City         │                                        │ EventType   │
│ Address      │                                        │ MaxPart...  │
│ HealthStatus │          ┌─────────────────┐          │ Budget      │
│ HealthNotes  │          │  Sponsorships   │          │ Notes       │
│ EducationSt..│          ├─────────────────┤          │ CreatedAt   │
│ SchoolName   │          │ Id (PK)         │          │ UpdatedAt   │
│ ClassLevel   │          │ OrphanId (FK)   │          └─────────────┘
│ GuardianName │◄─────────│ SponsorName     │
│ GuardianPh.. │          │ SponsorEmail    │
│ GuardianRe.. │          │ SponsorPhone    │
│ Sponsorship..│          │ Amount          │
│ PhotoUrl     │          │ Currency        │
│ Notes        │          │ Frequency       │
│ CreatedAt    │          │ StartDate       │
│ UpdatedAt    │          │ EndDate         │
└──────────────┘          │ IsActive        │
                          │ Notes           │
                          │ CreatedAt       │
                          │ UpdatedAt       │
                          └─────────────────┘
```

### Relationships

1. **Orphans ↔ Events** (Many-to-Many)
   - Junction table: `OrphanEvents`
   - An orphan can participate in multiple events
   - An event can have multiple orphans
   - Tracks attendance status and notes per participation

2. **Orphans → Sponsorships** (One-to-Many)
   - An orphan can have multiple sponsorships over time
   - Each sponsorship belongs to one orphan
   - Tracks sponsor details, amount, frequency, and dates

### Indexes (To be implemented)

- `Users.Email` (Unique Index)
- `Orphans.NationalId` (Unique Index, if not null)
- `Orphans.City` (Non-unique Index for filtering)
- `Orphans.SponsorshipStatus` (Non-unique Index for filtering)
- `Events.StartDate` (Non-unique Index for date range queries)
- `Events.EventType` (Non-unique Index for filtering)

## Authentication & Authorization

### JWT Token Structure

```json
{
  "sub": "user-guid",
  "email": "user@example.com",
  "name": "John Doe",
  "role": "Admin",
  "nbf": 1234567890,
  "exp": 1234571490,
  "iat": 1234567890,
  "iss": "OrphanManagementAPI",
  "aud": "OrphanManagementUI"
}
```

### Role-Based Permissions

| Feature | Admin | SocialWorker | Volunteer | Viewer |
|---------|-------|--------------|-----------|--------|
| Manage Users | ✓ | ✗ | ✗ | ✗ |
| Create/Edit Orphans | ✓ | ✓ | ✗ | ✗ |
| View Orphans | ✓ | ✓ | ✓ | ✓ |
| Create/Edit Events | ✓ | ✓ | ✗ | ✗ |
| View Events | ✓ | ✓ | ✓ | ✓ |
| Manage Attendance | ✓ | ✓ | ✓ | ✗ |
| View Dashboard | ✓ | ✓ | ✓ | ✓ |

## API Endpoints (Planned)

### Authentication
- `POST /api/auth/login` - User login
- `POST /api/auth/register` - Register new user (Admin only)
- `POST /api/auth/refresh` - Refresh access token
- `POST /api/auth/change-password` - Change password

### Users
- `GET /api/users` - List users (paginated, with filters)
- `GET /api/users/{id}` - Get user by ID
- `POST /api/users` - Create user
- `PUT /api/users/{id}` - Update user
- `DELETE /api/users/{id}` - Delete user
- `PATCH /api/users/{id}/toggle-status` - Activate/deactivate user

### Orphans
- `GET /api/orphans` - List orphans (paginated, with filters)
- `GET /api/orphans/{id}` - Get orphan by ID
- `POST /api/orphans` - Create orphan
- `PUT /api/orphans/{id}` - Update orphan
- `DELETE /api/orphans/{id}` - Delete orphan
- `POST /api/orphans/{id}/photo` - Upload orphan photo
- `GET /api/orphans/statistics/by-city` - Orphan count by city
- `GET /api/orphans/statistics/by-age-group` - Orphan count by age group

### Events
- `GET /api/events` - List events (paginated, with filters)
- `GET /api/events/{id}` - Get event by ID
- `POST /api/events` - Create event
- `PUT /api/events/{id}` - Update event
- `DELETE /api/events/{id}` - Delete event
- `POST /api/events/{id}/orphans` - Add orphans to event
- `DELETE /api/events/{eventId}/orphans/{orphanId}` - Remove orphan from event
- `PATCH /api/events/{eventId}/orphans/{orphanId}/attendance` - Update attendance
- `GET /api/events/upcoming` - Get upcoming events
- `GET /api/events/statistics/by-month` - Event count by month

### Dashboard
- `GET /api/dashboard/statistics` - Overall system statistics
- `GET /api/dashboard/orphan-demographics` - Orphan demographics charts
- `GET /api/dashboard/sponsorship-summary` - Sponsorship summary
- `GET /api/dashboard/upcoming-events` - Upcoming events summary

## Frontend Architecture (Angular 17+)

### Project Structure

```
orphan-management-ui/
├── src/
│   ├── app/
│   │   ├── core/                  # Singleton services, guards, interceptors
│   │   │   ├── guards/
│   │   │   │   ├── auth.guard.ts
│   │   │   │   └── role.guard.ts
│   │   │   ├── interceptors/
│   │   │   │   ├── auth.interceptor.ts      # Add JWT to requests
│   │   │   │   └── error.interceptor.ts     # Handle HTTP errors
│   │   │   ├── services/
│   │   │   │   ├── auth.service.ts
│   │   │   │   ├── storage.service.ts
│   │   │   │   └── notification.service.ts
│   │   │   └── models/
│   │   │       ├── user.model.ts
│   │   │       └── api-response.model.ts
│   │   ├── shared/                # Reusable components
│   │   │   ├── components/
│   │   │   │   ├── table/
│   │   │   │   ├── modal/
│   │   │   │   ├── file-upload/
│   │   │   │   └── loading-spinner/
│   │   │   ├── directives/
│   │   │   │   └── has-role.directive.ts
│   │   │   └── pipes/
│   │   │       ├── date-format.pipe.ts
│   │   │       └── age.pipe.ts
│   │   ├── features/              # Feature modules
│   │   │   ├── auth/
│   │   │   │   ├── login/
│   │   │   │   └── auth.service.ts
│   │   │   ├── dashboard/
│   │   │   │   ├── dashboard.component.ts
│   │   │   │   └── dashboard.service.ts
│   │   │   ├── users/
│   │   │   │   ├── user-list/
│   │   │   │   ├── user-form/
│   │   │   │   └── users.service.ts
│   │   │   ├── orphans/
│   │   │   │   ├── orphan-list/
│   │   │   │   ├── orphan-detail/
│   │   │   │   ├── orphan-form/
│   │   │   │   └── orphans.service.ts
│   │   │   └── events/
│   │   │       ├── event-list/
│   │   │       ├── event-detail/
│   │   │       ├── event-form/
│   │   │       └── events.service.ts
│   │   ├── app.component.ts       # Root component
│   │   ├── app.config.ts          # App configuration
│   │   └── app.routes.ts          # Route definitions
│   ├── assets/
│   │   ├── i18n/
│   │   │   ├── en.json
│   │   │   └── ar.json
│   │   └── images/
│   ├── environments/
│   │   ├── environment.ts
│   │   └── environment.prod.ts
│   └── styles/
│       ├── _variables.scss
│       ├── _rtl.scss              # RTL styles for Arabic
│       └── styles.scss            # Global styles
└── angular.json
```

### Key Features

1. **Standalone Components** (Angular 17+)
   - No NgModules, use standalone components
   - Direct imports in component metadata

2. **Route Guards**
   - `AuthGuard`: Check if user is authenticated
   - `RoleGuard`: Check if user has required role

3. **HTTP Interceptors**
   - `AuthInterceptor`: Add JWT token to all requests
   - `ErrorInterceptor`: Handle 401/403/500 errors globally

4. **State Management**
   - Simple service-based pattern with BehaviorSubjects
   - NgRx can be added later if needed

5. **Internationalization (i18n)**
   - ngx-translate for English/Arabic support
   - RTL layout switching for Arabic

6. **UI Framework**
   - Angular Material or Tailwind CSS
   - Responsive design (mobile-first)

## Security Considerations

### Backend Security

1. **Password Security**
   - BCrypt for password hashing (cost factor: 12)
   - Minimum password requirements enforced

2. **JWT Security**
   - Short token expiration (60 minutes)
   - Refresh token mechanism
   - Secure secret key (256-bit minimum)

3. **SQL Injection Prevention**
   - Parameterized queries via EF Core
   - Input validation with FluentValidation

4. **Authorization**
   - Role-based access control on every endpoint
   - `[Authorize(Roles = "Admin")]` attributes

5. **CORS**
   - Configured for specific origins only
   - No wildcard (*) in production

### Frontend Security

1. **XSS Prevention**
   - Angular's built-in sanitization
   - No `innerHTML` with untrusted data

2. **CSRF Protection**
   - Not needed for JWT-based auth (stateless)

3. **Token Storage**
   - Store JWT in memory or sessionStorage
   - Never in localStorage (XSS risk)

4. **HTTPS**
   - Always use HTTPS in production
   - Redirect HTTP to HTTPS

## Testing Strategy (To be implemented)

### Backend Tests

1. **Unit Tests**
   - Service layer business logic
   - Validators
   - xUnit + Moq

2. **Integration Tests**
   - Repository layer with in-memory database
   - API endpoints with TestServer

3. **Test Coverage Goal**
   - Minimum 80% code coverage
   - Focus on critical business logic

### Frontend Tests

1. **Unit Tests**
   - Services with HttpClientTestingModule
   - Components with isolated tests
   - Jasmine + Karma

2. **E2E Tests**
   - Critical user flows
   - Playwright or Cypress

## Deployment (To be implemented)

### Backend Deployment

- Docker containerization
- Environment-specific configurations
- Database migration on startup
- Health check endpoints

### Frontend Deployment

- Build for production: `ng build --configuration production`
- Serve with Nginx or Azure Static Web Apps
- Environment variables for API URL

### CI/CD Pipeline

- GitHub Actions or Azure DevOps
- Automated testing on PR
- Automated deployment to staging/production

## Performance Considerations

1. **Database Optimization**
   - Proper indexing on frequently queried fields
   - Pagination for large datasets
   - Query optimization (avoid N+1 problems)

2. **API Optimization**
   - Response caching where appropriate
   - Gzip compression
   - Async/await throughout

3. **Frontend Optimization**
   - Lazy loading for feature modules
   - OnPush change detection strategy
   - Image optimization
   - Service worker for PWA (optional)

## Future Enhancements

1. **Phase 2**
   - Advanced reporting (PDF generation)
   - Email notifications (welcome, password reset, event reminders)
   - Document management (upload files for orphans)
   - Audit log (track all changes)

2. **Phase 3**
   - Mobile app (React Native or Flutter)
   - Real-time chat support (SignalR)
   - AI-powered insights (predictive analytics)
   - Integration with payment gateways for donations

## Conclusion

This architecture provides a solid foundation for a scalable, maintainable, and secure orphan management system. The clean separation of concerns, comprehensive validation, and role-based security ensure that the application meets professional standards while remaining easy to extend and maintain.
