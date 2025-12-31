# Quick Run Guide - Orphan Management System

Get the system up and running in 5 minutes!

## Prerequisites

- .NET 8 SDK installed
- SQL Server or PostgreSQL running

## Quick Start (5 Steps)

### 1. Configure Database Connection

Edit `src/OrphanManagement.API/appsettings.json`:

**For SQL Server:**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=OrphanManagementDb;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "UsePostgreSQL": false
}
```

**For PostgreSQL:**
```json
{
  "ConnectionStrings": {
    "PostgreSQL": "Host=localhost;Port=5432;Database=OrphanManagementDb;Username=postgres;Password=yourpassword"
  },
  "UsePostgreSQL": true
}
```

### 2. Install EF Core Tools (if not already installed)

```bash
dotnet tool install --global dotnet-ef
```

### 3. Create Database and Run Migrations

```bash
cd src/OrphanManagement.API
dotnet ef migrations add InitialCreate --project ../OrphanManagement.Infrastructure
dotnet ef database update --project ../OrphanManagement.Infrastructure
```

### 4. Run the Application

```bash
dotnet run
```

### 5. Test the API

Open browser: `https://localhost:7001/swagger`

**Login Credentials:**
- Email: `admin@orphan.com`
- Password: `Admin@123`

## Pre-Seeded Data

The system automatically seeds the following data on first run:

### Users (6 accounts)
| Email | Password | Role |
|-------|----------|------|
| admin@orphan.com | Admin@123 | Admin |
| sarah.johnson@orphan.com | Social@123 | SocialWorker |
| michael.chen@orphan.com | Social@123 | SocialWorker |
| emma.williams@orphan.com | Volunteer@123 | Volunteer |
| david.brown@orphan.com | Volunteer@123 | Volunteer |
| lisa.anderson@orphan.com | Viewer@123 | Viewer |

### Orphans
8 orphans with complete profiles (names, ages, cities, education, health, guardians)

### Events
6 events including sports days, educational workshops, health camps, and celebrations

### OrphanEvents
Multiple orphan-event relationships with attendance tracking

## Quick Test with Swagger

1. Go to https://localhost:7001/swagger
2. Click **POST /api/Auth/login**
3. Click "Try it out"
4. Enter:
   ```json
   {
     "email": "admin@orphan.com",
     "password": "Admin@123"
   }
   ```
5. Click "Execute"
6. Copy the token from response
7. Click "Authorize" button at top
8. Enter: `Bearer YOUR_TOKEN_HERE`
9. Test any endpoint!

## Quick Test with cURL

```bash
# Login
curl -X POST "https://localhost:7001/api/Auth/login" \
  -H "Content-Type: application/json" \
  -d '{"email": "admin@orphan.com", "password": "Admin@123"}' -k

# Get Orphans (replace TOKEN)
curl -X GET "https://localhost:7001/api/Orphans" \
  -H "Authorization: Bearer YOUR_TOKEN" -k

# Get Events (replace TOKEN)
curl -X GET "https://localhost:7001/api/Events" \
  -H "Authorization: Bearer YOUR_TOKEN" -k

# Get Users (Admin only, replace TOKEN)
curl -X GET "https://localhost:7001/api/Users" \
  -H "Authorization: Bearer YOUR_TOKEN" -k
```

## API Endpoints Summary

### Auth
- `POST /api/Auth/login` - Login

### Orphans
- `GET /api/Orphans` - List orphans (paginated)
- `GET /api/Orphans/{id}` - Get orphan details
- `POST /api/Orphans` - Create orphan (Admin/SocialWorker)
- `PUT /api/Orphans/{id}` - Update orphan (Admin/SocialWorker)
- `DELETE /api/Orphans/{id}` - Delete orphan (Admin)

### Events
- `GET /api/Events` - List events (paginated)
- `GET /api/Events/{id}` - Get event details
- `POST /api/Events` - Create event (Admin/SocialWorker)
- `PUT /api/Events/{id}` - Update event (Admin/SocialWorker)
- `DELETE /api/Events/{id}` - Delete event (Admin)
- `POST /api/Events/{id}/participants` - Add orphans to event

### Users
- `GET /api/Users` - List users (Admin only)
- `POST /api/Users` - Create user (Admin only)
- `PUT /api/Users/{id}` - Update user (Admin only)
- `DELETE /api/Users/{id}` - Delete user (Admin only)

## Troubleshooting

**Can't connect to database?**
- Check database server is running
- Verify connection string in appsettings.json

**"Unauthorized" errors?**
- Login first to get JWT token
- Add token to Authorization header: `Bearer YOUR_TOKEN`

**Migration errors?**
- Delete Migrations folder in Infrastructure project
- Run migration commands again

**Port already in use?**
- Change ports in `src/OrphanManagement.API/Properties/launchSettings.json`

## Need More Details?

See the full [SETUP.md](SETUP.md) guide for:
- Detailed configuration options
- Troubleshooting guide
- Production deployment
- Security best practices
- Advanced features

## Quick Reset (Delete Everything and Start Over)

```bash
cd src/OrphanManagement.API
dotnet ef database drop --project ../OrphanManagement.Infrastructure --force
rm -rf ../OrphanManagement.Infrastructure/Migrations
dotnet ef migrations add InitialCreate --project ../OrphanManagement.Infrastructure
dotnet ef database update --project ../OrphanManagement.Infrastructure
dotnet run
```

---

**You're all set! The system is ready to use.** 🎉
