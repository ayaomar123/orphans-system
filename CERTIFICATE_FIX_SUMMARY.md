# Certificate Fix Summary

## Issue
Users encountering `ERR_CERT_AUTHORITY_INVALID` error when accessing the application at `https://localhost:7001`.

## Root Cause
ASP.NET Core development HTTPS certificate not trusted by the browser.

## Solution Implemented

### 1. Created Launch Profiles
**File**: `src/OrphanManagement.API/Properties/launchSettings.json` (NEW)

Added three launch profiles:
- **http**: HTTP-only on port 5000 (no certificate needed)
- **https**: HTTPS on port 7001 + HTTP on port 5000 (requires trusted cert)
- **IIS Express**: For Visual Studio users

### 2. Updated Program.cs
**File**: `src/OrphanManagement.API/Program.cs`

Changes:
- Made HTTPS redirection conditional in development
- Added helpful logging messages about certificate issues
- HTTPS redirection now respects `UseHttpsRedirection` configuration setting

### 3. Updated Development Settings
**File**: `src/OrphanManagement.API/appsettings.Development.json`

Added:
```json
"UseHttpsRedirection": false
```

This disables HTTPS redirection in development mode by default.

### 4. Created Documentation
**New Files**:
- `CERTIFICATE_FIX.md`: Comprehensive guide to fix certificate issues
- `QUICK_START_NO_CERTIFICATE_ISSUES.md`: Quick start using HTTP profile
- Updated `README.md`: Added references to new guides and HTTP profile usage

## How to Use

### Option 1: HTTP (Recommended for Development)
```bash
cd src/OrphanManagement.API
dotnet run --launch-profile http
```
Access at: http://localhost:5000/swagger

### Option 2: HTTPS (After Trusting Certificate)
```bash
# Trust the certificate first
dotnet dev-certs https --trust

# Then run with HTTPS
cd src/OrphanManagement.API
dotnet run --launch-profile https
```
Access at: https://localhost:7001/swagger

## Files Changed

### Created
1. `src/OrphanManagement.API/Properties/launchSettings.json`
2. `CERTIFICATE_FIX.md`
3. `QUICK_START_NO_CERTIFICATE_ISSUES.md`
4. `CERTIFICATE_FIX_SUMMARY.md` (this file)

### Modified
1. `src/OrphanManagement.API/Program.cs`
   - Made HTTPS redirection conditional
   - Added logging messages
   
2. `src/OrphanManagement.API/appsettings.Development.json`
   - Added `UseHttpsRedirection: false`
   
3. `README.md`
   - Updated Getting Started section
   - Added references to new documentation
   - Updated Troubleshooting section

## Benefits

✅ **No Certificate Errors**: HTTP profile works immediately without certificate setup
✅ **Flexible**: Both HTTP and HTTPS options available
✅ **Developer-Friendly**: Clear documentation and helpful logging
✅ **No Breaking Changes**: Existing HTTPS functionality preserved
✅ **Production-Safe**: Changes only affect development environment

## Testing

To verify the fix works:

1. **Test HTTP Profile**:
   ```bash
   cd src/OrphanManagement.API
   dotnet run --launch-profile http
   ```
   Open http://localhost:5000/swagger - Should work without certificate warnings

2. **Test HTTPS Profile** (if certificate trusted):
   ```bash
   cd src/OrphanManagement.API
   dotnet run --launch-profile https
   ```
   Open https://localhost:7001/swagger - Should work with trusted certificate

3. **Test Frontend Integration**:
   Update Angular environment to use `http://localhost:5000/api` and verify API calls work

## Rollback Instructions

If needed, to rollback these changes:

1. Delete `src/OrphanManagement.API/Properties/launchSettings.json`
2. Revert changes in `Program.cs`
3. Revert changes in `appsettings.Development.json`
4. Delete new documentation files

However, these changes are non-breaking and improve developer experience, so rollback should not be necessary.

## Future Considerations

- Consider adding a configuration option for default profile selection
- May want to document certificate setup for production environments
- Consider adding Docker support with pre-configured certificates
