# Changelog

All notable changes to PharmaDocs are documented here.  
Format based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/).

---

## [Unreleased]

### Planned

- JWT authentication (`POST /api/auth/login`)
- `AuthController` with login and token response
- Response DTOs for GET endpoints
- `SubmissionPackagesController` CRUD
- `DocumentRecordsController` CRUD

---

## [0.3.0] - 2026-05-16

### Added

- `ProductsController` with full CRUD: `GET`, `GET /{id}`, `POST`, `PUT /{id}`, `DELETE /{id}`
- `CreateProductDto` in Application layer — clean POST contract (no entity graph exposure)
- `UpdateProductDto` in Application layer — clean PUT contract
- JSON enum serialization configured — enums serialize as strings (e.g. `"Draft"`, `"NDS"`)

---

## [0.2.0] - 2026-05-16

### Added

- EF Core configured in `Program.cs` with Npgsql provider
- `PharmaDocsDbContext` registered with PostgreSQL connection string
- EF Core migrations generated and applied — all 6 tables created in `pharmadocs_db`
- Seed data: 2 users (Sarah Leblanc — RegAffairsOfficer, James Okafor — Viewer)
- Seed data: 2 products (Atorvastatin 20mg — Apotex Inc., Metformin 500mg — Teva Canada)
- `Microsoft.EntityFrameworkCore.Design` added to API project for `dotnet ef` CLI support
- `AddControllers()` and `MapControllers()` configured
- Swagger/OpenAPI confirmed working at `http://localhost:5046/swagger`

### Changed

- `Program.cs` updated from minimal API template to controller-based architecture

---

## [0.1.0] - 2026-05-14

### Added

- Clean Architecture solution structure (Domain, Application, Infrastructure, API)
- Core domain entities: `User`, `Product`, `DocumentRecord`, `SubmissionPackage`, `SubmissionDocument`, `AuditLog`
- Domain enums: `UserRole`, `DocumentType`, `DocumentStatus`, `SubmissionType`, `SubmissionStatus`
- NuGet packages: EF Core, Npgsql, JWT Bearer, BCrypt.Net, Swashbuckle, FluentValidation
- Project documentation: README, DOMAIN_CONTEXT, CHANGELOG
- `.gitignore` updated to exclude `appsettings.json` and `.lscache` files
