# Orphan Management System

A comprehensive full-stack web application for managing orphan information, sponsorships, events, and activities.

## 🏗️ Architecture

This application follows **Clean Architecture** principles with clear separation of concerns:

### Backend (.NET 8 Web API)
```
src/
├── OrphanManagement.Domain/          # Core domain entities and business rules
├── OrphanManagement.Application/     # Business logic, DTOs, interfaces, validators
├── OrphanManagement.Infrastructure/  # EF Core, Repositories, External services
└── OrphanManagement.API/             # Web API controllers, middleware, filters
```

### Frontend (Angular 17+)
```
orphan-management-ui/
└── src/app/
    ├── core/         # Singleton services, guards, interceptors
    ├── shared/       # Reusable components, pipes, directives
    └── features/     # Feature modules (auth, users, orphans, events, dashboard)
```

## 🗄️ Database Schema

### Core Entities

**Users**
- Id (GUID)
- FullName
- Email (unique)
- PasswordHash
- Role (Admin, SocialWorker, Volunteer, Viewer)
- Phone
- IsActive
- CreatedAt, UpdatedAt

**Orphans**
- Id (GUID)
- FirstName, LastName
- Gender
- DateOfBirth (Age calculated)
- NationalId (unique)
- City, Address
- HealthStatus, EducationStatus
- SchoolName, ClassLevel
- GuardianName, GuardianPhone, GuardianRelationship
- SponsorshipStatus
- PhotoUrl
- Notes
- CreatedAt, UpdatedAt

**Events**
- Id (GUID)
- Title, Description
- StartDate, EndDate
- Location, EventType
- MaxParticipants, Budget
- Notes
- CreatedAt, UpdatedAt

**OrphanEvents** (Many-to-Many)
- OrphanId, EventId
- AttendanceStatus
- Notes

**Sponsorships**
- Id (GUID)
- OrphanId (FK)
- SponsorName, Amount, Frequency
- StartDate, EndDate
- IsActive
- CreatedAt, UpdatedAt

## 🚀 Features

### Authentication & Authorization
- JWT-based authentication
- Role-based access control (Admin, SocialWorker, Volunteer, Viewer)
- Secure password hashing (BCrypt)
- Route guards and HTTP interceptors

### User Management (Admin only)
- CRUD operations for users
- Activate/Deactivate accounts
- Password reset functionality
- Search, filter, and pagination

### Orphan Management
- Complete CRUD operations
- Profile photo upload
- Advanced search and filtering
- Detailed profile view with tabs
- Guardian information management
- Sponsorship tracking

### Events Management
- CRUD operations for events
- Assign orphans to events
- Attendance tracking
- Date range filtering
- Participant management

### Dashboard
- Statistical overview
- Orphan demographics (by city, age group)
- Sponsorship statistics
- Upcoming events
- Interactive charts

### Multi-language Support
- English and Arabic
- RTL layout support for Arabic
- Language switcher in UI

## 🛠️ Technology Stack

### Backend
- **Framework**: ASP.NET Core 8.0
- **ORM**: Entity Framework Core 8.0
- **Database**: SQL Server / PostgreSQL
- **Authentication**: JWT Bearer Tokens
- **Validation**: FluentValidation
- **Mapping**: AutoMapper
- **Logging**: Serilog
- **API Documentation**: Swagger/OpenAPI

### Frontend
- **Framework**: Angular 17+ (Standalone Components)
- **UI Library**: Angular Material / Tailwind CSS
- **State Management**: Service-based pattern
- **Forms**: Reactive Forms
- **HTTP Client**: Angular HttpClient
- **Internationalization**: ngx-translate
- **Charts**: Chart.js / ngx-charts

## 📋 Prerequisites

### Backend
- .NET 8 SDK
- SQL Server 2019+ or PostgreSQL 13+
- Visual Studio 2022 or VS Code

### Frontend
- Node.js 18+ and npm
- Angular CLI 17+

## 🚦 Getting Started

### Backend Setup

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd orphan-management-system
   ```

2. **Update connection string**
   
   Edit `src/OrphanManagement.API/appsettings.json`:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=localhost;Database=OrphanManagementDb;Trusted_Connection=True;TrustServerCertificate=True;"
   }
   ```

3. **Run database migrations**
   ```bash
   cd src/OrphanManagement.API
   dotnet ef database update
   ```

4. **Run the API**
   ```bash
   dotnet run
   ```
   
   API will be available at `https://localhost:7001` and `http://localhost:5001`

5. **Access Swagger documentation**
   
   Navigate to `https://localhost:7001/swagger`

### Frontend Setup

1. **Navigate to the frontend directory**
   ```bash
   cd orphan-management-ui
   ```

2. **Install dependencies**
   ```bash
   npm install
   ```

3. **Update API URL**
   
   Edit `src/environments/environment.ts`:
   ```typescript
   export const environment = {
     production: false,
     apiUrl: 'https://localhost:7001/api'
   };
   ```

4. **Run the development server**
   ```bash
   ng serve
   ```
   
   Application will be available at `http://localhost:4200`

## 🔐 Default Users

After running migrations, you can seed default users:

| Email | Password | Role |
|-------|----------|------|
| admin@orphan.com | Admin@123 | Admin |
| social@orphan.com | Social@123 | SocialWorker |
| volunteer@orphan.com | Volunteer@123 | Volunteer |

## 🔒 Security Features

- **Password Hashing**: BCrypt with salt
- **JWT Tokens**: Short-lived access tokens with refresh capability
- **Role-based Authorization**: Endpoint protection by user role
- **Input Validation**: Both client-side and server-side
- **SQL Injection Prevention**: Parameterized queries via EF Core
- **XSS Prevention**: Angular's built-in sanitization
- **CORS**: Configured for specific origins
- **HTTPS**: Enforced in production

## 📁 Project Structure

### Backend Clean Architecture Layers

#### Domain Layer
- **Entities**: Core business entities
- **Enums**: Domain enumerations
- **Exceptions**: Domain-specific exceptions
- **Interfaces**: Domain service contracts
- **No dependencies** on other layers

#### Application Layer
- **DTOs**: Data Transfer Objects
- **Interfaces**: Service and repository contracts
- **Services**: Business logic implementation
- **Validators**: FluentValidation rules
- **Mappings**: AutoMapper profiles
- **Depends on**: Domain layer only

#### Infrastructure Layer
- **Persistence**: DbContext, Configurations
- **Repositories**: Data access implementation
- **Services**: External service implementations
- **Migrations**: EF Core migrations
- **Depends on**: Domain, Application layers

#### API Layer
- **Controllers**: HTTP endpoints
- **Middleware**: Exception handling, logging
- **Filters**: Action filters, authorization
- **Extensions**: Service registration
- **Depends on**: All layers

### Frontend Feature-based Structure

```
src/app/
├── core/
│   ├── guards/           # Route guards (auth, role-based)
│   ├── interceptors/     # HTTP interceptors (JWT, error handling)
│   ├── services/         # Singleton services (auth, storage)
│   └── models/           # Core interfaces and types
├── shared/
│   ├── components/       # Reusable components (table, modal, form controls)
│   ├── directives/       # Custom directives
│   ├── pipes/            # Custom pipes (date, currency)
│   └── validators/       # Custom form validators
└── features/
    ├── auth/             # Login, register, password reset
    ├── dashboard/        # Dashboard with statistics
    ├── users/            # User management (list, create, edit)
    ├── orphans/          # Orphan management (list, detail, create, edit)
    └── events/           # Event management (list, detail, create, edit)
```

## 🧪 Testing

### Backend Tests
```bash
cd tests
dotnet test
```

### Frontend Tests
```bash
cd orphan-management-ui
npm test                 # Unit tests
npm run e2e             # E2E tests
```

## 📚 API Documentation

Once the API is running, visit `/swagger` for interactive API documentation.

### Main Endpoints

#### Authentication
- `POST /api/auth/login` - Login
- `POST /api/auth/register` - Register (Admin only)
- `POST /api/auth/refresh` - Refresh token

#### Users
- `GET /api/users` - List users (with pagination)
- `GET /api/users/{id}` - Get user by ID
- `POST /api/users` - Create user (Admin)
- `PUT /api/users/{id}` - Update user (Admin)
- `DELETE /api/users/{id}` - Delete user (Admin)

#### Orphans
- `GET /api/orphans` - List orphans (with filters)
- `GET /api/orphans/{id}` - Get orphan details
- `POST /api/orphans` - Create orphan
- `PUT /api/orphans/{id}` - Update orphan
- `DELETE /api/orphans/{id}` - Delete orphan
- `POST /api/orphans/{id}/photo` - Upload photo

#### Events
- `GET /api/events` - List events
- `GET /api/events/{id}` - Get event details
- `POST /api/events` - Create event
- `PUT /api/events/{id}` - Update event
- `DELETE /api/events/{id}` - Delete event
- `POST /api/events/{id}/orphans` - Assign orphans
- `PUT /api/events/{eventId}/orphans/{orphanId}` - Update attendance

## 🌍 Internationalization

The application supports English and Arabic with RTL layout switching:

```typescript
// Switch language
this.translateService.use('ar'); // Arabic
this.translateService.use('en'); // English
```

## 🎨 UI Components

### Responsive Design
- Mobile-first approach
- Breakpoints for tablet and desktop
- Touch-friendly controls

### Key Components
- **Data Tables**: Sortable, filterable, paginated
- **Forms**: Reactive forms with validation
- **Modals**: For create/edit operations
- **Toast Notifications**: Success/error messages
- **Charts**: Visual data representation
- **File Upload**: Drag-and-drop photo upload

## 🔧 Configuration

### Backend Configuration (`appsettings.json`)
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "..."
  },
  "JwtSettings": {
    "Secret": "your-256-bit-secret",
    "Issuer": "OrphanManagementAPI",
    "Audience": "OrphanManagementUI",
    "ExpirationInMinutes": 60
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information"
    }
  }
}
```

### Frontend Configuration (`environment.ts`)
```typescript
export const environment = {
  production: false,
  apiUrl: 'https://localhost:7001/api',
  tokenKey: 'auth_token',
  refreshTokenKey: 'refresh_token'
};
```

## 📝 Development Guidelines

### Code Style
- Follow Clean Code principles
- Use meaningful names
- Keep methods small and focused
- Write self-documenting code
- Add comments only for complex logic

### Git Workflow
- Feature branches: `feat/feature-name`
- Bug fixes: `fix/bug-description`
- Commit messages: Follow conventional commits

### Best Practices
- Use async/await consistently
- Implement proper error handling
- Validate all inputs
- Use DTOs for API communication
- Implement logging for debugging
- Write unit tests for business logic

## 🐛 Troubleshooting

### Backend Issues

**Database connection fails**
- Verify SQL Server is running
- Check connection string in `appsettings.json`
- Ensure database exists or run migrations

**JWT authentication fails**
- Verify JWT secret in configuration
- Check token expiration settings
- Ensure proper Authorization header format

### Frontend Issues

**API calls fail with CORS errors**
- Check CORS configuration in backend
- Verify API URL in environment file

**Components not loading**
- Clear browser cache
- Check console for errors
- Verify all dependencies installed

## 📞 Support

For issues and questions:
- Create an issue in the repository
- Contact the development team

## 📄 License

This project is licensed under the MIT License.

## 👥 Contributors

- Development Team

## 🗺️ Roadmap

### Phase 1 (Current)
- ✅ Core CRUD operations
- ✅ Authentication and authorization
- ✅ Basic dashboard

### Phase 2 (Planned)
- [ ] Advanced reporting
- [ ] Email notifications
- [ ] Document management
- [ ] Mobile app (React Native)

### Phase 3 (Future)
- [ ] AI-powered insights
- [ ] Integration with external services
- [ ] Real-time chat support
- [ ] Advanced analytics

---

**Built with ❤️ for orphan care and management**
