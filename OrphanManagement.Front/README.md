# OrphanManagement Front (Angular)

This folder contains the Angular frontend for the Orphan Management System.

## Prerequisites
- Node.js 20+
- npm 10+

## Install
```bash
cd OrphanManagement.Front
npm install
```

## Run (development)
The backend API runs by default on `https://localhost:7001`.

```bash
npm start
```

This uses the included `proxy.conf.json` so calls to `/api/*` are proxied to the backend.

## Default credentials
- **Email**: `admin@orphan.com`
- **Password**: `Admin@123`

## Build
```bash
npm run build
```

## Notes
- The app uses **standalone components** (no NgModules).
- JWT is stored in `localStorage` and attached via an HTTP interceptor.

## Troubleshooting localStorage Token Issues

If you're experiencing issues with tokens not persisting in localStorage:

### 1. Check Browser Console
Open the browser Developer Tools (F12) and check the Console tab. The auth service now provides detailed logging:
- `AuthService initialized` - Service is loaded
- `localStorage is working correctly` - localStorage is accessible
- `Login successful, storing auth data` - Token is being saved
- `Verified: Auth data persisted in localStorage` - Token is still there after 100ms
- `WARNING: Auth data was cleared...` - Token disappeared (external issue)

### 2. Check localStorage in DevTools
1. Open Developer Tools (F12)
2. Go to Application tab (Chrome) or Storage tab (Firefox)
3. Click on "Local Storage" → your domain
4. Look for the key `om.auth`
5. Verify it contains your token and other auth data

### 3. Common Causes
- **Browser Privacy Mode**: Incognito/Private browsing may block localStorage
- **Browser Extensions**: Ad blockers or privacy extensions may clear storage
- **Token Expiration**: Check if `expiresAt` is set too short (should be 60 minutes)
- **401 Errors**: Any API call returning 401 will trigger logout and clear the token
- **localStorage Quota**: Browser storage might be full (rare)

### 4. Test localStorage Manually
Open Browser Console and run:
```javascript
localStorage.setItem('test', 'value');
console.log(localStorage.getItem('test'));
localStorage.removeItem('test');
```

If this doesn't work, your browser has localStorage disabled or restricted.

### 5. Check Token Expiration
The token includes an `expiresAt` field. Check the backend configuration:
- Look for `JwtSettings:ExpirationInMinutes` in `appsettings.json`
- Default should be 60 minutes, not 1 second

### 6. Monitor Network Requests
1. Open Developer Tools → Network tab
2. Log in and watch for:
   - `/api/Auth/login` - Should return 200 with token
   - Any subsequent API calls returning 401
3. If you see immediate 401 errors after login, the token might be invalid

### 7. Use the Debug Tool
Open `debug-localstorage.html` in your browser to run automated localStorage diagnostics:
1. Navigate to: `http://localhost:4200/debug-localstorage.html` (when dev server is running)
2. Or open the file directly in your browser: `OrphanManagement.Front/debug-localstorage.html`
3. Click "Run All Tests" to check:
   - localStorage availability
   - Read/write functionality
   - Data persistence over time
   - Current auth token status and expiration
4. Use "Monitor Changes" to watch for unexpected localStorage modifications
