# Quick Start Guide - Orphan Management System

This guide will help you get the Orphan Management System up and running in minutes.

## 📋 Prerequisites

Before you begin, ensure you have:

- ✅ **.NET 8 SDK** installed ([Download](https://dotnet.microsoft.com/download/dotnet/8.0))
- ✅ **SQL Server** (or **PostgreSQL**) installed and running
- ✅ **Node.js 18+** and **npm** ([Download](https://nodejs.org/))
- ✅ **Git** for version control
- ✅ A code editor (**VS Code** or **Visual Studio 2022** recommended)

## 🚀 Step 1: Verify .NET Installation

```bash
dotnet --version
# Should show 8.0.x or higher
```

If .NET 8 is not installed, download and install it from the link above.

## 🗄️ Step 2: Configure Database

### Option A: SQL Server (Default)

1. Ensure SQL Server is running
2. Open `src/OrphanManagement.API/appsettings.json`
3. Update the connection string:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=OrphanManagementDb;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

### Option B: PostgreSQL

1. Ensure PostgreSQL is running
2. Open `src/OrphanManagement.API/appsettings.json`
3. Update the connection string and set `UsePostgreSQL` to `true`:

```json
{
  "UsePostgreSQL": true,
  "ConnectionStrings": {
    "PostgreSQL": "Host=localhost;Database=OrphanManagementDb;Username=postgres;Password=your_password"
  }
}
```

## 🔧 Step 3: Run Database Migrations

Open a terminal in the project root directory and run:

```bash
# Navigate to the API project
cd src/OrphanManagement.API

# Install EF Core tools (if not already installed)
dotnet tool install --global dotnet-ef

# Create initial migration
dotnet ef migrations add InitialCreate --project ../OrphanManagement.Infrastructure

# Apply migration to database
dotnet ef database update
```

This will:
- Create the database
- Create all tables
- Seed a default admin user

## ▶️ Step 4: Run the Backend API

```bash
# Still in src/OrphanManagement.API directory
dotnet run
```

You should see output like:

```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:7001
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5001
```

**✅ Backend is now running!**

## 🧪 Step 5: Test the API

### Option A: Use Swagger UI (Recommended)

1. Open your browser
2. Navigate to: `https://localhost:7001/swagger`
3. You'll see the interactive API documentation
4. Click on **"Authorize"** button at the top
5. Use the **POST /api/auth/login** endpoint with these credentials:

```json
{
  "email": "admin@orphan.com",
  "password": "Admin@123"
}
```

6. Copy the returned `token` value
7. Click **"Authorize"** again and enter: `Bearer YOUR_TOKEN_HERE`
8. Now you can test all API endpoints!

### Option B: Use cURL

```bash
# Login to get JWT token
curl -X POST https://localhost:7001/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"admin@orphan.com","password":"Admin@123"}' \
  -k

# Use the returned token in subsequent requests
curl -X GET https://localhost:7001/api/orphans \
  -H "Authorization: Bearer YOUR_TOKEN_HERE" \
  -k
```

### Option C: Use Postman

1. Create a new POST request to `https://localhost:7001/api/auth/login`
2. Set Header: `Content-Type: application/json`
3. Set Body (raw JSON):
```json
{
  "email": "admin@orphan.com",
  "password": "Admin@123"
}
```
4. Send the request
5. Copy the `token` from the response
6. For all other requests, add Header: `Authorization: Bearer YOUR_TOKEN_HERE`

## 🎨 Step 6: Run the Frontend (Optional - To Be Implemented)

The Angular frontend is not yet implemented. To create it:

```bash
# Navigate to project root
cd /path/to/project

# Create Angular project
ng new orphan-management-ui --standalone --routing --style=scss

# Navigate into the project
cd orphan-management-ui

# Install dependencies
npm install @angular/material @angular/cdk @ngx-translate/core @ngx-translate/http-loader

# Update environment file with API URL
# Edit: src/environments/environment.ts

# Run development server
ng serve
```

The app will be available at `http://localhost:4200`

## 🔐 Default User Accounts

After database seeding, you have:

| Email | Password | Role | Access Level |
|-------|----------|------|--------------|
| admin@orphan.com | Admin@123 | Admin | Full access to all features |

## 📝 Common Commands

### Backend

```bash
# Restore NuGet packages
dotnet restore

# Build the solution
dotnet build

# Run the API
cd src/OrphanManagement.API
dotnet run

# Run with hot reload (watch mode)
dotnet watch run

# Create a new migration
dotnet ef migrations add MigrationName --project ../OrphanManagement.Infrastructure

# Update database
dotnet ef database update

# Remove last migration
dotnet ef migrations remove --project ../OrphanManagement.Infrastructure

# Drop database (careful!)
dotnet ef database drop --project ../OrphanManagement.Infrastructure
```

### Frontend (When Implemented)

```bash
# Install dependencies
npm install

# Run development server
ng serve

# Build for production
ng build --configuration production

# Run tests
npm test

# Run linting
ng lint
```

## 🐛 Troubleshooting

### Issue: "Unable to connect to database"

**Solution**: 
- Verify SQL Server/PostgreSQL is running
- Check connection string in `appsettings.json`
- Ensure database server allows local connections

### Issue: "JWT Secret not configured"

**Solution**:
- Open `src/OrphanManagement.API/appsettings.json`
- Ensure `JwtSettings:Secret` is set and is at least 32 characters long

### Issue: "Cannot create database"

**Solution**:
- Ensure you have permissions to create databases
- Try running with elevated privileges
- Check firewall settings

### Issue: "CORS error when calling API from Angular"

**Solution**:
- Open `src/OrphanManagement.API/appsettings.json`
- Add your frontend URL to `CorsOrigins` array:
```json
{
  "CorsOrigins": [
    "http://localhost:4200",
    "https://localhost:4200"
  ]
}
```

### Issue: "Port already in use"

**Solution**:
- Change ports in `src/OrphanManagement.API/Properties/launchSettings.json`
- Or kill the process using the port:
  - Windows: `netstat -ano | findstr :7001` then `taskkill /PID <PID> /F`
  - Linux/Mac: `lsof -i :7001` then `kill -9 <PID>`

## 📚 Next Steps

1. ✅ **Explore the API**: Try different endpoints in Swagger
2. ✅ **Create Test Data**: Add some orphans and events via the API
3. ✅ **Implement Services**: Complete UserService, OrphanService, EventService
4. ✅ **Add Controllers**: Implement UsersController, OrphansController, EventsController
5. ✅ **Build Frontend**: Create the Angular application
6. ✅ **Add Tests**: Write unit and integration tests
7. ✅ **Deploy**: Prepare for production deployment

## 📖 Additional Resources

- **Full Documentation**: See `README.md`
- **Architecture Guide**: See `ARCHITECTURE.md`
- **Implementation Guide**: See `IMPLEMENTATION_GUIDE.md`
- **Project Status**: See `PROJECT_SUMMARY.md`

## 🆘 Getting Help

If you encounter issues:

1. Check the troubleshooting section above
2. Review the error logs in `logs/` directory
3. Check the console output for error messages
4. Create an issue in the repository with:
   - Error message
   - Steps to reproduce
   - Environment details (.NET version, OS, database)

## 🎉 Success!

If everything is working, you should be able to:

- ✅ Login via Swagger with admin credentials
- ✅ See the default admin user
- ✅ Call protected API endpoints with JWT token
- ✅ View API documentation in Swagger UI

**Congratulations! Your Orphan Management System backend is now running!** 🎊

---

**Quick Test Checklist:**

- [ ] .NET 8 SDK installed
- [ ] Database server running
- [ ] Connection string configured
- [ ] Migrations applied successfully
- [ ] API running on https://localhost:7001
- [ ] Swagger UI accessible
- [ ] Login successful with default credentials
- [ ] JWT token received
- [ ] Protected endpoints accessible with token

**Happy Coding! 💻**
