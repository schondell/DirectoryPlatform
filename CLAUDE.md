# DirectoryPlatform

Generic classified ads platform (petitesannonces.ch style) — Swiss market focus.

## Architecture
Clean Architecture with 5 layers:
- **Core** — Entities, Enums, Interfaces (no dependencies)
- **Contracts** — DTOs, Service interfaces (refs Core)
- **Application** — Business logic, AutoMapper, Validators (refs Core, Contracts)
- **Infrastructure** — EF Core, Repositories, External services (refs Core, Application)
- **API** — Controllers, Program.cs, Config (refs all)

## Build Commands
### Backend
```bash
cd backend/DirectoryPlatform
dotnet restore
dotnet build
dotnet run --project src/API  # Runs on http://localhost:5100
```

### Frontend
```bash
cd frontend
npm install
npm start  # Runs on http://localhost:4200, proxies /api to :5100
```

### Docker (dev infrastructure)
```bash
docker compose -f docker-compose.dev.yml up -d  # Postgres :5433, Redis :6379, MinIO :9000
```

### Docker (full stack)
```bash
docker compose up --build
```

## Key Patterns
- **Data-driven filtering**: AttributeDefinition defines per-category dynamic attributes, ListingAttribute stores values, ListingRepository.GetFilteredAsync builds dynamic EF queries from attr[slug]=value query params
- **Seed data**: Swiss cantons/cities, 32 category trees matching petitesannonces.ch (Automobiles, Real Estate, Computing, Telephony, Employment, Animals, Weapons, etc.) with attribute definitions for 16 key subcategories
- **Auth**: JWT + refresh tokens, PBKDF2+SHA256 password hashing
- **API Docs**: Scalar at /scalar/v1
- **Engagement**: Likes, Follows, Page Views, Visitor tracking per listing
- **Boosts**: Standard/Premium/Featured boost types with daily pricing
- **Financial**: Invoice/Payment system with Swiss VAT (8.1%), auto invoice numbering (INV-YYYY-XXXX)
- **KPI Dashboard**: Professional user KPIs (views, likes, followers, messages, response rate, category performance)
- **Admin Dashboard**: System metrics, visitor analytics, revenue overview, system health

## Subscription Tiers
- Free (3 listings, 3 photos)
- Standard CHF 9.90/mo (15 listings, 10 photos, basic analytics)
- Premium CHF 29.90/mo (50 listings, 20 photos, featured, 3 boost credits)
- Business CHF 79.90/mo (unlimited listings, 30 photos, KPI dashboard, API access, 10 boost credits)

## Database
PostgreSQL with snake_case naming convention. Connection string in appsettings.json, default port 5433.

## Test Users
- admin@admin.com / Admin123! (Admin role)
- user@user.com / User123! (User role)
