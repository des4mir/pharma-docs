# Changelog

All notable changes to PharmaDocs are documented here.  
Format based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/).

---

## [Unreleased]

### Planned

- `SubmissionPackagesController` CRUD
- Status workflow validation
- Audit log writing on status changes

---

## [0.7.0] - 2026-05-28

### Fixed

- `createdById` now resolved from JWT claims (`ClaimTypes.NameIdentifier`) instead of request body
- Removed `CreatedById` from `CreateProductDto` and `CreateDocumentDto` — client can no longer spoof creator identity

### Added

- `backend/docs/` folder with developer testing resources:
  - `API-TESTING-GUIDE.md` — endpoint reference, seeded data, expected results
  - `test-api.ps1` — automated PowerShell script covering all 9 test scenarios
  - `curl-commands.txt` — manual curl reference for both roles

---

## [0.6.0] - 2026-05-27

### Added

- `DocumentsController` with full CRUD: `GET`, `GET /{id}`, `GET /by-product/{productId}`, `POST`, `PUT /{id}`, `DELETE /{id}`
- `DocumentResponseDto`, `CreateDocumentDto`, `UpdateDocumentDto` in Application layer
- `POST/PUT/DELETE` restricted to `RegAffairsOfficer` role; `GET` open to both roles
- `Label` added to `DocumentType` enum
- `DocumentType` and `DocumentStatus` enums serialize as strings in responses
- Seed data: 2 `DocumentRecord` rows for local testing

---

## [0.5.0] - 2026-05-25

### Added

- `ProductResponseDto` — clean API response contract, no EF navigation properties exposed
- Private `ToDto()` helper on `ProductsController` for consistent entity-to-DTO mapping
- Swagger schemas now reflect DTOs only

---

## [0.4.1] - 2026-05-24

### Added

- Swagger XML comments and `ProducesResponseType` annotations on all `ProductsController` endpoints

---

## [0.4.0] - 2026-05-20

### Added

- `[Authorize]` at controller level on `ProductsController`
- `[Authorize(Roles = "RegAffairsOfficer")]` on `POST`, `PUT`, `DELETE` endpoints
- Swagger Bearer authentication UI configured

---

## [0.3.0] - 2026-05-18

### Added

- `JwtTokenService` in Infrastructure — generates signed HS256 JWT tokens with standard claim types
- `LoginDto` and `AuthResponseDto` in Application layer
- `AuthController` with `POST /api/auth/login` — validates credentials with BCrypt, returns JWT
- `System.IdentityModel.Tokens.Jwt` 8.4.0 package added to Infrastructure
- `UseAuthentication()` middleware registered before `UseAuthorization()` in pipeline
- JWT Bearer authentication fully configured in `Program.cs`

---

## [0.2.0] - 2026-05-16

### Added

- `ProductsController` with full CRUD: `GET`, `GET /{id}`, `POST`, `PUT /{id}`, `DELETE /{id}`
- `CreateProductDto` in Application layer — clean POST contract (no entity graph exposure)
- `UpdateProductDto` in Application layer — clean PUT contract
- JSON enum serialization configured — enums serialize as strings (e.g. `"Draft"`, `"NDS"`)
- EF Core configured in `Program.cs` with Npgsql provider
- `PharmaDocsDbContext` registered with PostgreSQL connection string
- EF Core migrations generated and applied — all 6 tables created in `pharmadocs_db`
- Seed data: 2 users (Sarah Leblanc — RegAffairsOfficer, James Okafor — Viewer)
- Seed data: 2 products (Atorvastatin 20mg — Apotex Inc., Metformin 500mg — Teva Canada)
- `Microsoft.EntityFrameworkCore.Design` added to API project for `dotnet ef` CLI support
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
