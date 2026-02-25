# DirectoryPlatform

A generic, data-driven directory/marketplace platform built with .NET 10 and Angular 21.

## Features
- Category-based listings with dynamic, filterable attributes
- Hierarchical categories and regions (Swiss cantons + cities)
- JWT authentication with role-based access control
- User messaging and notifications
- Review system with moderation
- Subscription tiers
- S3-compatible media storage (MinIO)
- Server-side rendering (Angular SSR)
- Admin dashboard with listing approval workflow

## Quick Start

### Prerequisites
- .NET 10 SDK
- Node.js 20+
- Docker & Docker Compose
- PostgreSQL 16 (or use Docker)

### Development Setup

1. Start infrastructure:
```bash
docker compose -f docker-compose.dev.yml up -d
```

2. Run backend:
```bash
cd backend/DirectoryPlatform
dotnet run --project src/API
```

3. Run frontend:
```bash
cd frontend
npm install
npm start
```

### URLs
- Frontend: http://localhost:4200
- Backend API: http://localhost:5100
- API Docs (Scalar): http://localhost:5100/scalar/v1
- MinIO Console: http://localhost:9001

## Tech Stack
- **Backend:** .NET 10, PostgreSQL 16, EF Core, JWT, AutoMapper, FluentValidation, Serilog
- **Frontend:** Angular 21, Syncfusion, @ngx-translate, Leaflet, Jest
- **Infrastructure:** Docker Compose, Nginx, Redis, MinIO

## License
MIT
