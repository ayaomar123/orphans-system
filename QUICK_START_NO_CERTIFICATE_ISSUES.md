# Quick Start Guide (No Certificate Issues)

This guide will help you run the application without encountering certificate errors.

## Start the Application with HTTP

```bash
cd src/OrphanManagement.API
dotnet run --launch-profile http
```

That's it! The application will start on **http://localhost:5000** without any certificate warnings.

## Access the Application

Once the application is running, open your browser and navigate to:

- **Swagger API Documentation**: http://localhost:5000/swagger
- **API Base URL**: http://localhost:5000/api

## Test the API

1. Go to http://localhost:5000/swagger
2. Find the **POST /api/auth/login** endpoint
3. Click "Try it out"
4. Use these default credentials:
   ```json
   {
     "email": "admin@orphan.com",
     "password": "Admin@123"
   }
   ```
5. You'll receive a JWT token that you can use to test other endpoints

## Frontend Configuration

If you're building a frontend application, use this API URL:

```typescript
// environment.development.ts
export const environment = {
  production: false,
  apiUrl: 'http://localhost:5000/api'
};
```

## Why HTTP Instead of HTTPS?

- ✅ **No certificate warnings** - Works immediately
- ✅ **Faster development** - No need to trust certificates
- ✅ **Same functionality** - All API features work identically
- ✅ **Browser-friendly** - No security warnings

For local development, HTTP is perfectly fine. Use HTTPS when deploying to production.

## Need HTTPS?

If you specifically need HTTPS for testing (e.g., testing secure cookies, PWA features), see [CERTIFICATE_FIX.md](./CERTIFICATE_FIX.md) for instructions on trusting the development certificate.

## Troubleshooting

### Port 5000 Already in Use

Change the port in `Properties/launchSettings.json`:

```json
{
  "profiles": {
    "http": {
      "applicationUrl": "http://localhost:5001"
    }
  }
}
```

### Application Redirects to HTTPS

The application is configured to **not redirect to HTTPS in development mode** by default.

If you still experience redirects, check `appsettings.Development.json`:

```json
{
  "UseHttpsRedirection": false
}
```

## What Was Changed?

To fix the certificate issue, the following changes were made:

1. **Created `Properties/launchSettings.json`**
   - Added `http` profile for HTTP-only development
   - Added `https` profile for HTTPS with certificate
   
2. **Updated `Program.cs`**
   - Made HTTPS redirection optional in development
   - Added helpful logging messages
   
3. **Updated `appsettings.Development.json`**
   - Set `UseHttpsRedirection: false` for development

4. **No Breaking Changes**
   - All existing functionality works exactly the same
   - You can still use HTTPS if you trust the certificate
   - Production deployments are unaffected

## Summary

**Before:**
- ❌ Certificate error: `ERR_CERT_AUTHORITY_INVALID`
- ❌ Had to manually trust certificates
- ❌ Complex setup for new developers

**After:**
- ✅ Run with `dotnet run --launch-profile http`
- ✅ Access via http://localhost:5000
- ✅ No certificate errors
- ✅ Works immediately

Happy coding! 🚀
