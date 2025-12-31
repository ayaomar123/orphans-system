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
