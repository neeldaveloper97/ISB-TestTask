# Property Inventory System (ISB001)

Full-stack property inventory system: ASP.NET Core 8 Web API + Angular 17 UI.
Manages properties, contacts (owners), ownership history, price history, and a dashboard
showing every ownership change with USD conversion via [frankfurter.app](https://frankfurter.app).

## Architecture

```
PropertyInventory/
├── PropertyInventory.Domain/          Entity models
├── PropertyInventory.Application/     DTOs, service interfaces + logic, repository interfaces
├── PropertyInventory.Infrastructure/  EF Core DbContext, configs, repositories, CurrencyService, seed, migrations
├── PropertyInventory.API/             Controllers, Program.cs, global exception handler
└── property-inventory-ui/             Angular 17 standalone app (Angular Material)
```

## Prerequisites

- .NET 8 SDK
- SQL Server LocalDB (ships with Visual Studio / `sqllocaldb`)
- Node.js 18+ and npm
- (optional) Angular CLI: `npm i -g @angular/cli`

## Backend

```bash
cd PropertyInventory.API
dotnet restore
dotnet ef database update      # or just `dotnet run` — migrations apply automatically on startup
dotnet run
# API at http://localhost:5000  (Swagger UI at http://localhost:5000/swagger)
```

The database is created, migrated, and seeded automatically on first startup
(3 contacts, 2 properties, 3 ownership records per the functional spec).

## Frontend

```bash
cd property-inventory-ui
npm install
ng serve            # or: npm start
# UI at http://localhost:4200
```

The UI reads the API base URL from `src/environments/environment.ts` (`http://localhost:5000/api`).

## API Surface

| Method | Route | Description |
|--------|-------|-------------|
| GET | `/api/properties` | List (pagination + filter by name/address/price range) |
| GET | `/api/properties/{id}` | Single property w/ ownership + price history |
| POST | `/api/properties` | Create one |
| POST | `/api/properties/bulk` | Create many |
| PUT | `/api/properties/{id}` | Update one (price change adds a price-history record) |
| PUT | `/api/properties/bulk` | Update many |
| DELETE | `/api/properties/{id}` | Hard delete |
| POST | `/api/properties/{id}/ownership` | Assign/transfer owner (closes prior ownership) |
| GET | `/api/contacts` | List (pagination + filter) |
| GET | `/api/contacts/{id}` | Single contact w/ ownership history |
| POST | `/api/contacts` `/bulk` | Create one / many |
| PUT | `/api/contacts/{id}` `/bulk` | Update one / many |
| GET | `/api/dashboard` | All ownership changes w/ USD conversion, newest first |

## Notes / Assumptions

- No authentication (per spec).
- Hard delete only (soft delete out of scope).
- Currency conversion uses historical rates on the date of sale; if the API is unavailable,
  `soldAtPriceUsd` is `null` and the UI shows "N/A". USD amounts pass through unchanged.
- Bulk endpoints process items sequentially.
- `DateOfRegistration` defaults to today when omitted on create.
