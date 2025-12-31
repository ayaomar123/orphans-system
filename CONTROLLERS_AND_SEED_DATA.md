# Controllers and Seed Data Implementation

This document describes the newly created controllers and seed data for the Orphan Management System.

## Created Files

### 1. Controllers

#### OrphansController.cs
**Location**: `src/OrphanManagement.API/Controllers/OrphansController.cs`

**Endpoints**:
- `GET /api/Orphans` - Get all orphans with pagination and advanced filtering
  - Filters: searchTerm, city, gender, sponsorshipStatus, educationStatus, minAge, maxAge
- `GET /api/Orphans/{id}` - Get orphan by ID
- `POST /api/Orphans` - Create new orphan (Admin/SocialWorker only)
- `PUT /api/Orphans/{id}` - Update orphan (Admin/SocialWorker only)
- `DELETE /api/Orphans/{id}` - Delete orphan (Admin only)
- `POST /api/Orphans/{id}/photo` - Upload orphan photo (Admin/SocialWorker only)
- `GET /api/Orphans/statistics/by-city` - Get orphan statistics by city
- `GET /api/Orphans/statistics/by-age-group` - Get orphan statistics by age group

**Features**:
- Comprehensive logging for all operations
- Role-based authorization
- File upload with validation (images only)
- Proper error handling and exception management
- XML documentation for Swagger

#### EventsController.cs
**Location**: `src/OrphanManagement.API/Controllers/EventsController.cs`

**Endpoints**:
- `GET /api/Events` - Get all events with pagination and filtering
  - Filters: searchTerm, eventType, startDate, endDate, location
- `GET /api/Events/{id}` - Get event by ID
- `POST /api/Events` - Create new event (Admin/SocialWorker only)
- `PUT /api/Events/{id}` - Update event (Admin/SocialWorker only)
- `DELETE /api/Events/{id}` - Delete event (Admin only)
- `POST /api/Events/{id}/participants` - Add multiple orphans to event (Admin/SocialWorker only)
- `DELETE /api/Events/{id}/participants/{orphanId}` - Remove orphan from event
- `PATCH /api/Events/{id}/participants/{orphanId}/attendance` - Update attendance status
- `GET /api/Events/upcoming` - Get upcoming events (configurable count)
- `GET /api/Events/statistics/by-month` - Get event statistics by month for a year

**Features**:
- Participant management (add/remove orphans)
- Attendance tracking with notes
- Comprehensive logging
- Role-based authorization
- Proper error handling

#### UsersController.cs
**Location**: `src/OrphanManagement.API/Controllers/UsersController.cs`

**Endpoints**:
- `GET /api/Users` - Get all users with pagination and filtering (Admin only)
  - Filters: searchTerm, role
- `GET /api/Users/{id}` - Get user by ID (Admin only)
- `GET /api/Users/by-email/{email}` - Get user by email (Admin only)
- `POST /api/Users` - Create new user (Admin only)
- `PUT /api/Users/{id}` - Update user (Admin only)
- `DELETE /api/Users/{id}` - Delete user (Admin only)
- `PATCH /api/Users/{id}/status` - Toggle user active/inactive status (Admin only)

**Features**:
- Full user management (Admin only)
- Account status management
- Email-based user lookup
- Comprehensive logging
- Strict role-based authorization

### 2. Seed Data

#### DbSeeder.cs
**Location**: `src/OrphanManagement.Infrastructure/Data/DbSeeder.cs`

**Functionality**:
Automatically populates the database with initial data on first run.

**Seeded Data**:

##### Users (6 accounts)
| Role | Email | Password | Name |
|------|-------|----------|------|
| Admin | admin@orphan.com | Admin@123 | Admin User |
| SocialWorker | sarah.johnson@orphan.com | Social@123 | Sarah Johnson |
| SocialWorker | michael.chen@orphan.com | Social@123 | Michael Chen |
| Volunteer | emma.williams@orphan.com | Volunteer@123 | Emma Williams |
| Volunteer | david.brown@orphan.com | Volunteer@123 | David Brown |
| Viewer | lisa.anderson@orphan.com | Viewer@123 | Lisa Anderson |

##### Orphans (8 records)
Complete profiles including:
- Personal details (names, DOB, gender, NationalID)
- Location (cities: Cairo, Alexandria, Giza, Mansoura)
- Health information (status, notes, medical conditions)
- Education (status, school names, class levels)
- Guardian information (names, phones, relationships)
- Sponsorship status (Sponsored, Pending, NotSponsored)
- Additional notes

Sample orphans:
1. Ahmed Hassan (Male, 9 years old, Cairo) - Sponsored
2. Mariam Ali (Female, 11 years old, Alexandria) - Pending
3. Omar Mohamed (Male, 8 years old, Giza) - Not Sponsored, needs medical attention
4. Zainab Ibrahim (Female, 12 years old, Cairo) - Sponsored
5. Youssef Mahmoud (Male, 10 years old, Mansoura) - Pending
6. Nour Karim (Female, 7 years old, Alexandria) - Not Sponsored
7. Hassan Farid (Male, 13 years old, Cairo) - Sponsored
8. Salma Mustafa (Female, 9 years old, Giza) - Pending, vision problems

##### Events (6 events)
Diverse event types with complete information:

1. **Annual Sports Day** (Recreational)
   - 15 days from now, Cairo Sports Complex
   - Budget: $5,000, Max participants: 50

2. **Educational Workshop: Science for Kids** (Educational)
   - 7 days from now, Cairo Science Center
   - Budget: $3,000, Max participants: 30

3. **Health Checkup Camp** (Medical)
   - 5 days from now, Community Health Center
   - Budget: $8,000, Max participants: 40

4. **Eid Celebration** (Cultural)
   - 20 days from now, Community Center Hall
   - Budget: $10,000, Max participants: 100

5. **Art & Crafts Workshop** (Educational)
   - 10 days from now, Arts Center, Alexandria
   - Budget: $2,000, Max participants: 25

6. **Summer Camp 2024** (Recreational)
   - 60 days from now, Camp Green Valley
   - Budget: $25,000, Max participants: 60
   - Week-long, overnight accommodation

##### OrphanEvents (Relationships)
- First 5 orphans registered for first 3 events
- Various attendance statuses (Registered, Attended, Absent)
- Sample notes for some registrations

## Integration with Program.cs

The seeding is integrated into the application startup process:

```csharp
// Seed database with initial data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        
        // Apply migrations automatically
        await context.Database.MigrateAsync();
        
        // Seed comprehensive data
        await DbSeeder.SeedAsync(context);
        
        Log.Information("Database initialized and seeded successfully");
    }
    catch (Exception ex)
    {
        Log.Error(ex, "An error occurred while migrating or seeding the database");
    }
}
```

**Features**:
- Idempotent (only seeds if database is empty)
- Automatic on first run
- Uses BCrypt for password hashing
- Creates realistic test data
- Proper entity relationships

## Documentation Files

### SETUP.md
**Location**: `/SETUP.md`

Comprehensive setup guide including:
- Prerequisites
- Database configuration (SQL Server and PostgreSQL)
- JWT configuration
- Migration commands
- Seed data information
- API endpoint documentation
- User roles and permissions
- Troubleshooting guide
- Production deployment checklist

### QUICK_RUN.md
**Location**: `/QUICK_RUN.md`

Quick start guide (5 steps) including:
- Minimal setup instructions
- Quick database configuration
- Pre-seeded data credentials
- Quick testing with Swagger and cURL
- Endpoint summary
- Quick troubleshooting
- Quick reset commands

## Usage Instructions

### 1. Running Migrations

```bash
cd src/OrphanManagement.API
dotnet ef migrations add InitialCreate --project ../OrphanManagement.Infrastructure
dotnet ef database update --project ../OrphanManagement.Infrastructure
```

### 2. Running the Application

```bash
cd src/OrphanManagement.API
dotnet run
```

The seed data will be automatically applied on first run.

### 3. Testing the Controllers

#### Using Swagger UI:
1. Navigate to `https://localhost:7001/swagger`
2. Login via `/api/Auth/login` with `admin@orphan.com` / `Admin@123`
3. Copy the JWT token
4. Click "Authorize" and enter `Bearer YOUR_TOKEN`
5. Test any endpoint

#### Using cURL:
```bash
# Login
curl -X POST "https://localhost:7001/api/Auth/login" \
  -H "Content-Type: application/json" \
  -d '{"email": "admin@orphan.com", "password": "Admin@123"}' -k

# Get Orphans
curl -X GET "https://localhost:7001/api/Orphans?pageNumber=1&pageSize=10" \
  -H "Authorization: Bearer YOUR_TOKEN" -k

# Get Events
curl -X GET "https://localhost:7001/api/Events" \
  -H "Authorization: Bearer YOUR_TOKEN" -k

# Get Users (Admin only)
curl -X GET "https://localhost:7001/api/Users" \
  -H "Authorization: Bearer YOUR_TOKEN" -k
```

## Role-Based Access Control

Each controller implements proper authorization:

| Endpoint Type | Admin | SocialWorker | Volunteer | Viewer |
|--------------|-------|--------------|-----------|--------|
| View All Data | ✅ | ✅ | ✅ | ✅ |
| Create Orphan | ✅ | ✅ | ❌ | ❌ |
| Update Orphan | ✅ | ✅ | ❌ | ❌ |
| Delete Orphan | ✅ | ❌ | ❌ | ❌ |
| Create Event | ✅ | ✅ | ❌ | ❌ |
| Update Event | ✅ | ✅ | ❌ | ❌ |
| Delete Event | ✅ | ❌ | ❌ | ❌ |
| Update Attendance | ✅ | ✅ | ✅ | ❌ |
| Manage Users | ✅ | ❌ | ❌ | ❌ |

## Next Steps

1. **Implement Service Layer**: The controllers depend on service interfaces that need implementation:
   - `OrphanService` implementing `IOrphanService`
   - `EventService` implementing `IEventService`
   - `UserService` implementing `IUserService`

2. **Register Services**: Uncomment service registrations in `Program.cs`:
   ```csharp
   builder.Services.AddScoped<IOrphanService, OrphanService>();
   builder.Services.AddScoped<IEventService, EventService>();
   builder.Services.AddScoped<IUserService, UserService>();
   ```

3. **Create Frontend**: Build Angular application to consume the API

4. **Add Unit Tests**: Create test projects for controllers and services

5. **Deploy**: Follow production deployment guide in SETUP.md

## Notes

- All passwords are hashed using BCrypt
- JWT tokens expire after 24 hours (configurable)
- Database is automatically created and migrated on first run
- Seed data is idempotent (won't duplicate if run multiple times)
- All endpoints return consistent `ApiResponse<T>` format
- Comprehensive logging is implemented throughout
- File uploads are validated for security

## Support

For detailed information, see:
- [SETUP.md](SETUP.md) - Complete setup and configuration guide
- [QUICK_RUN.md](QUICK_RUN.md) - Quick start guide
- [ARCHITECTURE.md](ARCHITECTURE.md) - System architecture
- [IMPLEMENTATION_GUIDE.md](IMPLEMENTATION_GUIDE.md) - Implementation details
