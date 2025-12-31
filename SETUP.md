# Orphan Management System - Setup Guide

This guide will walk you through setting up and running the Orphan Management System from scratch.

## Prerequisites

- **.NET 8 SDK** (Download from https://dotnet.microsoft.com/download)
- **SQL Server** or **PostgreSQL** database
- **IDE**: Visual Studio 2022, Visual Studio Code, or JetBrains Rider (optional but recommended)
- **Git** (for version control)

## Project Structure

```
OrphanManagement/
├── src/
│   ├── OrphanManagement.API/          # Web API controllers and startup
│   ├── OrphanManagement.Application/  # DTOs, interfaces, validators
│   ├── OrphanManagement.Domain/       # Entities, enums, exceptions
│   └── OrphanManagement.Infrastructure/ # EF Core, repositories, services
└── SETUP.md (this file)
```

## Step 1: Clone or Navigate to the Project

```bash
cd /path/to/OrphanManagement
```

## Step 2: Configure Database Connection

### Option A: SQL Server (Default)

1. Open `src/OrphanManagement.API/appsettings.json`
2. Update the connection string:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=OrphanManagementDb;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "UsePostgreSQL": false
}
```

### Option B: PostgreSQL

1. Open `src/OrphanManagement.API/appsettings.json`
2. Update the connection string:

```json
{
  "ConnectionStrings": {
    "PostgreSQL": "Host=localhost;Port=5432;Database=OrphanManagementDb;Username=postgres;Password=yourpassword"
  },
  "UsePostgreSQL": true
}
```

### Important: JWT Configuration

Ensure JWT settings are configured in `appsettings.json`:

```json
{
  "JwtSettings": {
    "Secret": "YourSuperSecretKeyThatIsAtLeast256BitsLong!ChangeThisInProduction",
    "Issuer": "OrphanManagementAPI",
    "Audience": "OrphanManagementClient",
    "ExpirationInMinutes": 1440
  }
}
```

> **Note**: The Secret must be at least 32 characters (256 bits). Change this in production!

## Step 3: Restore NuGet Packages

Navigate to the solution directory and restore all dependencies:

```bash
cd src/OrphanManagement.API
dotnet restore
```

Or restore for the entire solution:

```bash
dotnet restore OrphanManagement.sln
```

## Step 4: Create Database Migrations

The project uses Entity Framework Core Code-First approach. Create the initial migration:

```bash
cd src/OrphanManagement.API
dotnet ef migrations add InitialCreate --project ../OrphanManagement.Infrastructure
```

### If `dotnet ef` is not installed:

```bash
dotnet tool install --global dotnet-ef
```

## Step 5: Apply Database Migrations

Apply the migrations to create the database schema:

```bash
cd src/OrphanManagement.API
dotnet ef database update --project ../OrphanManagement.Infrastructure
```

This will:
- Create the database if it doesn't exist
- Create all tables (Users, Orphans, Events, OrphanEvents, Sponsorships)
- Apply all entity configurations

## Step 6: Seed Initial Data

The application automatically seeds data on startup. The seed data includes:

### Users (6 accounts):
- **Admin**: admin@orphan.com / Admin@123
- **Social Worker**: sarah.johnson@orphan.com / Social@123
- **Social Worker**: michael.chen@orphan.com / Social@123
- **Volunteer**: emma.williams@orphan.com / Volunteer@123
- **Volunteer**: david.brown@orphan.com / Volunteer@123
- **Viewer**: lisa.anderson@orphan.com / Viewer@123

### Orphans (8 records):
- Sample orphans with complete profiles from various cities (Cairo, Alexandria, Giza, Mansoura)
- Various ages, genders, education levels, and sponsorship statuses

### Events (6 events):
- Annual Sports Day
- Educational Workshop: Science for Kids
- Health Checkup Camp
- Eid Celebration
- Art & Crafts Workshop
- Summer Camp 2024

### OrphanEvents (Relationships):
- Multiple orphans registered for various events
- Different attendance statuses (Confirmed, Pending, Attended)

> **Note**: Data is seeded automatically when you run the application for the first time.

## Step 7: Run the Application

Start the API:

```bash
cd src/OrphanManagement.API
dotnet run
```

Or with hot reload (recommended for development):

```bash
dotnet watch run
```

The API will start at:
- **HTTPS**: https://localhost:7001
- **HTTP**: http://localhost:5001
- **Swagger UI**: https://localhost:7001/swagger

## Step 8: Test the API

### Using Swagger UI

1. Open your browser and navigate to: `https://localhost:7001/swagger`
2. Click on **POST /api/Auth/login**
3. Click "Try it out"
4. Enter credentials:
   ```json
   {
     "email": "admin@orphan.com",
     "password": "Admin@123"
   }
   ```
5. Click "Execute"
6. Copy the JWT token from the response
7. Click the "Authorize" button at the top
8. Enter: `Bearer YOUR_TOKEN_HERE`
9. Now you can test all protected endpoints!

### Using cURL

#### Login:
```bash
curl -X POST "https://localhost:7001/api/Auth/login" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "admin@orphan.com",
    "password": "Admin@123"
  }' -k
```

#### Get Orphans (with token):
```bash
curl -X GET "https://localhost:7001/api/Orphans?pageNumber=1&pageSize=10" \
  -H "Authorization: Bearer YOUR_TOKEN_HERE" -k
```

### Using Postman

1. Import the API into Postman (or create requests manually)
2. Login via `POST /api/Auth/login` with admin credentials
3. Copy the token from the response
4. Add Authorization header: `Bearer YOUR_TOKEN_HERE`
5. Test other endpoints

## Step 9: Available Endpoints

### Authentication
- `POST /api/Auth/login` - User login
- `POST /api/Auth/validate-token` - Validate JWT token

### Orphans
- `GET /api/Orphans` - Get all orphans (paginated, with filters)
- `GET /api/Orphans/{id}` - Get orphan by ID
- `POST /api/Orphans` - Create new orphan (Admin/SocialWorker only)
- `PUT /api/Orphans/{id}` - Update orphan (Admin/SocialWorker only)
- `DELETE /api/Orphans/{id}` - Delete orphan (Admin only)
- `POST /api/Orphans/{id}/photo` - Upload orphan photo (Admin/SocialWorker only)
- `GET /api/Orphans/statistics/by-city` - Get statistics by city
- `GET /api/Orphans/statistics/by-age-group` - Get statistics by age group

### Events
- `GET /api/Events` - Get all events (paginated, with filters)
- `GET /api/Events/{id}` - Get event by ID
- `POST /api/Events` - Create new event (Admin/SocialWorker only)
- `PUT /api/Events/{id}` - Update event (Admin/SocialWorker only)
- `DELETE /api/Events/{id}` - Delete event (Admin only)
- `POST /api/Events/{id}/participants` - Add orphans to event (Admin/SocialWorker only)
- `DELETE /api/Events/{id}/participants/{orphanId}` - Remove orphan from event
- `PATCH /api/Events/{id}/participants/{orphanId}/attendance` - Update attendance
- `GET /api/Events/upcoming` - Get upcoming events
- `GET /api/Events/statistics/by-month` - Get statistics by month

### Users
- `GET /api/Users` - Get all users (Admin only)
- `GET /api/Users/{id}` - Get user by ID (Admin only)
- `GET /api/Users/by-email/{email}` - Get user by email (Admin only)
- `POST /api/Users` - Create new user (Admin only)
- `PUT /api/Users/{id}` - Update user (Admin only)
- `DELETE /api/Users/{id}` - Delete user (Admin only)
- `PATCH /api/Users/{id}/status` - Toggle user status (Admin only)

## Step 10: User Roles and Permissions

The system has four user roles with different permission levels:

| Role | Permissions |
|------|------------|
| **Admin** | Full access to all operations (CRUD on all resources) |
| **SocialWorker** | Create/Update orphans and events, view all data |
| **Volunteer** | Update attendance, view all data |
| **Viewer** | Read-only access to all data |

## Troubleshooting

### Issue: "Cannot find migration"
**Solution**: Run the migration command again:
```bash
dotnet ef migrations add InitialCreate --project ../OrphanManagement.Infrastructure
```

### Issue: "Connection failed"
**Solution**: 
1. Verify database server is running
2. Check connection string in `appsettings.json`
3. Ensure firewall allows connections

### Issue: "JWT Secret error"
**Solution**: Ensure `JwtSettings:Secret` in appsettings.json is at least 32 characters long

### Issue: "Unauthorized" on API calls
**Solution**: 
1. Login first to get a JWT token
2. Add Authorization header: `Bearer YOUR_TOKEN`
3. Ensure token hasn't expired (24 hours by default)

### Issue: Service implementation not found
**Solution**: The service implementations (OrphanService, EventService, UserService) need to be created in `OrphanManagement.Infrastructure/Services/`. Once created, uncomment the service registrations in `Program.cs`.

## Database Management Commands

### Create a new migration:
```bash
cd src/OrphanManagement.API
dotnet ef migrations add MigrationName --project ../OrphanManagement.Infrastructure
```

### Apply migrations:
```bash
dotnet ef database update --project ../OrphanManagement.Infrastructure
```

### Rollback to a specific migration:
```bash
dotnet ef database update PreviousMigrationName --project ../OrphanManagement.Infrastructure
```

### Remove last migration (if not applied):
```bash
dotnet ef migrations remove --project ../OrphanManagement.Infrastructure
```

### Drop entire database:
```bash
dotnet ef database drop --project ../OrphanManagement.Infrastructure
```

## Development Tips

### Enable hot reload:
```bash
dotnet watch run
```

### Run with specific environment:
```bash
# Development
dotnet run --environment Development

# Production
dotnet run --environment Production
```

### Check API health:
Open browser to `https://localhost:7001/swagger` - if Swagger loads, API is running correctly.

### View logs:
Logs are written to:
- Console output
- `logs/orphan-management-YYYYMMDD.txt` files

## Next Steps

1. ✅ Database created and migrated
2. ✅ Seed data loaded
3. ✅ API running
4. **Implement service layer** (OrphanService, EventService, UserService)
5. **Create frontend application** (Angular)
6. **Deploy to production environment**

## Production Deployment

Before deploying to production:

1. **Update JWT Secret** in appsettings.Production.json
2. **Configure production database** connection string
3. **Set appropriate CORS origins**
4. **Enable HTTPS** with valid SSL certificate
5. **Configure logging** (e.g., Azure Application Insights, ELK stack)
6. **Set up CI/CD pipeline** (GitHub Actions, Azure DevOps)
7. **Configure environment variables** for sensitive data
8. **Perform security audit**

## Support

For issues or questions:
- Check the project documentation in `ARCHITECTURE.md`, `IMPLEMENTATION_GUIDE.md`
- Review the code comments and XML documentation
- Check the TODO.md file for known issues and planned features

## License

[Your License Here]
