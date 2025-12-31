# Fix: ERR_CERT_AUTHORITY_INVALID on Localhost

## Problem
When accessing the application at `https://localhost:7001`, you encounter:
```
Your connection is not private
net::ERR_CERT_AUTHORITY_INVALID
```

This happens because the ASP.NET Core development HTTPS certificate is not trusted by your browser.

## Solutions

### Solution 1: Trust the Development Certificate (Recommended for HTTPS)

**Windows:**
```bash
cd src/OrphanManagement.API
dotnet dev-certs https --clean
dotnet dev-certs https --trust
```

**macOS:**
```bash
cd src/OrphanManagement.API
dotnet dev-certs https --clean
dotnet dev-certs https --trust
```

**Linux:**
```bash
cd src/OrphanManagement.API
dotnet dev-certs https --clean
dotnet dev-certs https --trust

# If trust fails on Linux, manually export and import:
dotnet dev-certs https -ep ~/localhost.crt --format PEM
sudo cp ~/localhost.crt /usr/local/share/ca-certificates/
sudo update-ca-certificates

# For Firefox on Linux:
certutil -d sql:$HOME/.pki/nssdb -A -t "P,," -n localhost -i ~/localhost.crt
```

After running these commands, restart your browser and try accessing the application again.

### Solution 2: Use HTTP Instead (Quick Fix for Development)

If you want to bypass certificate issues during development, use HTTP instead:

```bash
cd src/OrphanManagement.API
dotnet run --launch-profile http
```

The application will be available at:
- **API**: http://localhost:5000
- **Swagger**: http://localhost:5000/swagger

### Solution 3: Browser Workaround (Not Recommended)

You can bypass the warning temporarily:
1. On the certificate error page, click **Advanced**
2. Click **Proceed to localhost (unsafe)**

**Note**: This is not recommended for production or when handling sensitive data.

## Configuration Files Updated

### launchSettings.json (NEW)
A new `Properties/launchSettings.json` file has been created with three profiles:
- **http**: Runs on `http://localhost:5000` (no certificate needed)
- **https**: Runs on `https://localhost:7001` and `http://localhost:5000`
- **IIS Express**: For Visual Studio users

### How to Choose Profile

**Using dotnet CLI:**
```bash
# HTTP only (no certificate issues)
dotnet run --launch-profile http

# HTTPS (requires trusted certificate)
dotnet run --launch-profile https

# Default (uses https profile)
dotnet run
```

**Using Visual Studio:**
Select the profile from the dropdown next to the Run button.

**Using Visual Studio Code:**
The default profile (https) will be used unless specified in launch.json.

## Update Frontend Configuration

If using HTTP (port 5000), update your Angular app's API URL:

**environment.ts / environment.development.ts:**
```typescript
export const environment = {
  production: false,
  apiUrl: 'http://localhost:5000/api'  // Changed from https://localhost:7001
};
```

**appsettings.json** already includes both HTTP and HTTPS CORS origins:
```json
"CorsOrigins": [
  "http://localhost:4200",
  "https://localhost:4200"
]
```

## Verification

After fixing the certificate or switching to HTTP:

1. Start the API:
   ```bash
   cd src/OrphanManagement.API
   dotnet run --launch-profile http
   ```

2. Open your browser and navigate to:
   - http://localhost:5000/swagger

3. You should see the Swagger UI without any certificate warnings.

## Default Credentials

To test the API after fixing the certificate issue:
- **Email**: admin@orphan.com
- **Password**: Admin@123

## Troubleshooting

### Certificate Trust Fails on Windows
Run Command Prompt or PowerShell as Administrator and try again.

### Certificate Trust Fails on macOS
You may need to enter your system password when prompted.

### Certificate Trust Fails on Linux
Linux doesn't have a built-in trust mechanism like Windows/macOS. Use the manual export method shown above or use the HTTP profile.

### Port Already in Use
```bash
# Check what's using the port
netstat -ano | findstr :5000    # Windows
lsof -i :5000                    # macOS/Linux

# Kill the process or change the port in launchSettings.json
```

### Still Getting Certificate Errors After Trust
1. Clear browser cache and SSL state
2. Restart your browser completely
3. Try in incognito/private mode
4. Regenerate the certificate:
   ```bash
   dotnet dev-certs https --clean
   dotnet dev-certs https --trust
   ```

## Recommended Approach

**For Development:**
- Use the **http** profile (`dotnet run --launch-profile http`)
- Access via http://localhost:5000
- No certificate issues
- Faster to get started

**For Production-like Testing:**
- Use the **https** profile after trusting the certificate
- Access via https://localhost:7001
- Tests HTTPS redirects and secure configurations

## Summary

The certificate error has been addressed by:
1. ✅ Created `launchSettings.json` with both HTTP and HTTPS profiles
2. ✅ HTTP profile available at http://localhost:5000 (no certificate needed)
3. ✅ HTTPS profile available at https://localhost:7001 (requires trusted cert)
4. ✅ Documentation for trusting the development certificate
5. ✅ CORS already configured for both HTTP and HTTPS

**Quick Start (No Certificate Issues):**
```bash
cd src/OrphanManagement.API
dotnet run --launch-profile http
```
Then open http://localhost:5000/swagger in your browser.
