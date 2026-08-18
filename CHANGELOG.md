# Changelog

All notable changes to PharmaDocs are documented here.  
Format based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/).

---

## [Unreleased]

### Planned

- Enforce submission status transition validation rules (backend)
- Add global exception middleware (backend)
- Wire up FluentValidation on DTOs (package already referenced, no validators yet)
- Complete Swagger annotations for remaining endpoints
- Product create/edit form pages (frontend)
- Documents feature: list, detail, and create pages (frontend)
- Submission package board / status workflow UI (frontend)
- `AuditLogDrawer` timeline component (frontend)
- Dashboard/home page (frontend)
- Add Docker/docker-compose and deployment setup for Railway and Vercel

---

## [0.12.0] - 2026-08-16

### Added

- Products feature on the frontend:
  - `Product` / `CreateProductDto` types mirroring backend DTOs field-for-field (nullable `din`/`npn`)
  - `lib/session.ts` — reads a non-httpOnly "session hint" cookie server-side to expose `getSession()` and `isRegAffairsOfficer()` for UI-only role gating
  - `lib/products.ts` — typed data layer (list, get-by-id, create, archive), all routed through `apiFetch`
  - `/products` — Server Component list page: table view, role-gated "New Product" link, empty state
  - `/products/[id]` — dynamic detail page, calls `notFound()` on a 404 from the API
- Authenticated shell on the frontend:
  - `LogoutButton` (Client Component) — POSTs to the logout route and redirects to `/login`
  - `NavBar` (Server Component) — shows the signed-in user's name/role and a logout control, or a "Sign in" link when logged out
  - Root layout renders `NavBar` on every page
  - Login route now also sets a non-secret "session hint" cookie (`pharmadocs_session`) alongside the httpOnly JWT cookie; logout route clears both

### Fixed

- `ProductsController.Create` now rejects duplicate active products sharing the same DIN (`400 Bad Request`), preventing accidental duplicate entries from repeated form/test submissions

---

## [0.11.0] - 2026-08-03

### Added

- Next.js 16 App Router project scaffolded at `frontend/pharma-docs-web` — TypeScript strict mode, Tailwind CSS v4, ESLint
- `lib/types.ts` — TypeScript types mirroring backend DTOs and enums (`LoginDto`, `AuthResponseDto`, `SessionUser`, role/status/type unions)
- `lib/api.ts` — server-only typed `apiFetch()` client wrapper; attaches the JWT from the httpOnly cookie as a Bearer token on every backend call
- `app/api/auth/login/route.ts` and `app/api/auth/logout/route.ts` — Route Handlers that proxy authentication to the backend; the JWT is stored in an httpOnly cookie and never reaches client-side JavaScript
- `/login` page (Client Component) with a working sign-in form
- `src/proxy.ts` — Next.js 16's renamed middleware; optimistic auth guard that redirects unauthenticated requests to `/login`

---

## [0.10.0] - 2026-07-04

### Added

- `PATCH /api/products/{id}/unarchive`, `PATCH /api/documents/{id}/unarchive`, `PATCH /api/submissionpackages/{id}/unarchive` — restore soft-deleted (archived) resources; returns `404` if the resource is not currently archived
- `.github/copilot-instructions.md` workspace configuration for AI-assisted development

### Fixed

- Added missing `InProgress` value to the `SubmissionStatus` enum (between `Draft` and `Submitted`)
- Corrected seed data comment — seeded password is `Demo1234!`
- Aligned `API-TESTING-GUIDE.md`, `curl-commands.txt`, and `test-api.ps1` with the current v0.10.0 endpoint set and seeded document references

Backend reached feature-complete status as of this release — active development moved to the frontend.

---

## [0.9.0] - 2026-06-24

### Added

- `AuditLogsController` — dedicated audit log controller with a single query endpoint:
  - `GET /api/auditlogs?entityType={type}` — retrieve all audit entries for a given entity type
  - `GET /api/auditlogs?entityType={type}&entityId={id}` — narrow results to a specific entity
  - `entityType` is **required**; valid values: `Product`, `DocumentRecord`, `SubmissionPackage` — omitting it or passing an invalid value returns `400 Bad Request`
  - `entityId` is optional; omitting it returns all entries for the given type
  - Results ordered by `timestamp` descending
  - Access: both roles (RegAffairsOfficer and Viewer)
- `CreatedByName` field added to `ProductResponseDto` and `DocumentResponseDto` — surfaces actor identity without exposing internal user IDs
- `Status` field added to `CreateDocumentDto` — allows specifying document status at creation time (defaults to `Draft` if omitted)

### Changed

- `ProductResponseDto` and `DocumentResponseDto` updated to include `createdByName` in response bodies

### Removed

- `GET /api/submissionpackages/{id}/auditlog` — per-package audit sub-route removed; superseded by the dedicated `GET /api/auditlogs?entityType=SubmissionPackage&entityId={id}` endpoint on `AuditLogsController`

### Fixed

- Removed ghost `UserId` columns from `Products` and `Documents` tables — these were previously generated by EF Core due to incorrect navigation property configuration in `PharmaDocsDbContext`

---

## [0.8.0] - 2026-06-06

### Added

- `SubmissionPackagesController` with full CRUD and workflow support:
  - `GET /api/submissionpackages` — list all active packages
  - `GET /api/submissionpackages/{id}` — retrieve single package
  - `GET /api/submissionpackages/by-product/{productId}` — filter by product
  - `GET /api/submissionpackages/{id}/auditlog` — retrieve full audit history
  - `POST /api/submissionpackages` — create new package (RegAffairsOfficer only)
  - `PUT /api/submissionpackages/{id}` — update package details (RegAffairsOfficer only)
  - `PATCH /api/submissionpackages/{id}/status` — update status with audit logging (RegAffairsOfficer only)
  - `PATCH /api/submissionpackages/{id}/archive` — soft-delete package (blocked if Submitted/UnderReview - RegAffairsOfficer only)
- `CreateSubmissionPackageDto`, `UpdateSubmissionPackageDto`, `UpdateSubmissionStatusDto`, `SubmissionPackageResponseDto`, and `AuditLogResponseDto` in the Application layer
- Archive endpoints (`PATCH /{id}/archive`) on all three resource controllers (Products, Documents, Submission Packages) with soft delete and audit trail support
- Audit log expansion:
  - `Action` field captures operation type (`Created`, `Updated`, `Archived`, `StatusChanged`)
  - `OldValues` and `NewValues` capture entity snapshots before and after changes
  - `OldStatus` and `NewStatus` track status transitions with `SubmissionPackageId` linkage
  - `ChangedByName` preserves actor identity for historical accountability
- Seed data: 1 `SubmissionPackage` (NDS package for Atorvastatin)

### Changed

- `DELETE` operations on resources replaced with `PATCH /{id}/archive` for soft delete
- All write operations (`POST`, `PUT`, `PATCH`) now require the `RegAffairsOfficer` role
- Archive operations return `204 No Content` instead of an entity response
- Audit logging now captures detailed snapshots and actor information

### Fixed

- `PharmaDocsDbContext` comments now explain composite keys, enum conversions, delete behaviors, and seed data consistency

---

## [0.7.1] - 2026-06-01

### Changed

- Updated architecture documentation to describe the four-layer Clean Architecture flow.
- Corrected README setup notes to match the real development port and local appsettings filename.
- Aligned API testing docs with the current seeded document type and endpoint behavior.
- Replaced stale `Label` references in seeded-data documentation with `CertificateOfAnalysis`.

### Fixed

- Clarified that the default local API endpoint is `http://localhost:5046`.
- Fixed documentation drift between seeded data, PowerShell tests, and curl examples.
- Updated document seed references to match the current domain enum set.

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
