# Architecture

PharmaDocs is a two-part system: a four-project Clean Architecture backend and a
Next.js App Router frontend that consumes it exclusively through a typed API client.

---

## Backend Layers

| Layer              | Project                     | Responsibility                                                                                                                                                                                                                              |
| ------------------ | --------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| **Domain**         | `PharmaDocs.Domain`         | Core entities (`User`, `Product`, `DocumentRecord`, `SubmissionPackage`, `SubmissionDocument`, `AuditLog`) and enums (`UserRole`, `DocumentType`, `DocumentStatus`, `SubmissionType`, `SubmissionStatus`). No dependencies on other layers. |
| **Application**    | `PharmaDocs.Application`    | API contracts and DTOs only — `LoginDto`, `AuthResponseDto`, `Create/Update/*ResponseDto` for Products, Documents, and SubmissionPackages, `UpdateSubmissionStatusDto`, `AuditLogResponseDto`. Depends only on Domain.                      |
| **Infrastructure** | `PharmaDocs.Infrastructure` | `PharmaDocsDbContext`, EF Core / PostgreSQL configuration, migrations, seed data, `JwtTokenService`. Depends on Domain and Application.                                                                                                     |
| **API**            | `PharmaDocs.API`            | ASP.NET Core controllers (`AuthController`, `ProductsController`, `DocumentsController`, `SubmissionPackagesController`, `AuditLogsController`), middleware pipeline, Swagger config, JWT Bearer setup. Depends on all lower layers.        |

---

## Backend Dependency Flow

```
HTTP Request
│
▼
PharmaDocs.API (controllers, auth middleware, Swagger)
│
▼
PharmaDocs.Application (DTOs, request/response contracts)
│
▼
PharmaDocs.Infrastructure (DbContext, JwtTokenService, migrations)
│
▼
PharmaDocs.Domain (entities, enums — no external dependencies)
```

Dependency direction is always inward — Domain knows nothing about the layers above it.

---

## Frontend Architecture

Next.js 16 App Router project at `frontend/pharma-docs-web/src`, organized as:

| Area                    | Path                                                                           | Responsibility                                                                                                                                                                                                                                                             |
| ----------------------- | ------------------------------------------------------------------------------ | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| **Pages**               | `app/**/page.tsx`                                                              | Server Components by default; `"use client"` only where hooks/browser APIs are needed (e.g. the login form, `LogoutButton`).                                                                                                                                               |
| **Auth Route Handlers** | `app/api/auth/login`, `.../logout`                                             | Proxy authentication to the backend. Set/clear the httpOnly JWT cookie and the non-httpOnly session-hint cookie. The browser never calls the backend directly.                                                                                                             |
| **Auth guard**          | `src/proxy.ts`                                                                 | Next.js 16's renamed middleware. Optimistic check for the JWT cookie's presence; redirects to `/login` when absent. Not a security boundary — the backend re-validates the JWT on every request.                                                                           |
| **API client**          | `lib/api.ts`                                                                   | Server-only typed `apiFetch()` wrapper. Reads the JWT from the httpOnly cookie and attaches it as a Bearer token. All backend calls go through this — no component calls `fetch` against the backend directly.                                                             |
| **Session helper**      | `lib/session.ts`                                                               | Reads the non-httpOnly "session hint" cookie (`pharmadocs_session`) to expose `getSession()` / `isRegAffairsOfficer()` for Server Component UI gating. Carries no secret and grants no access — the backend independently enforces roles via `[Authorize(Roles = "...")]`. |
| **Types**               | `lib/types.ts`                                                                 | TypeScript interfaces mirroring backend DTOs and enums field-for-field.                                                                                                                                                                                                    |
| **Feature data layers** | `lib/products.ts` (and future `lib/documents.ts`, `lib/submissionPackages.ts`) | Typed functions per resource (list, get-by-id, create, archive, ...), each a thin wrapper over `apiFetch`.                                                                                                                                                                 |
| **Shared components**   | `components/`                                                                  | `NavBar` (Server Component, auth-aware), `LogoutButton` (Client Component), future `StatusBadge`, `AuditLogDrawer`, `ArchiveButton`.                                                                                                                                       |

### Full-Stack Request Flow

```
Browser
│
▼
Next.js Route Handler / Server Component  (proxy.ts guards the request first)
│  — reads httpOnly JWT cookie, calls apiFetch()
▼
ASP.NET Core API  (PharmaDocs.API)
│  — [Authorize] / [Authorize(Roles = "...")] re-validates the JWT on every request
▼
PostgreSQL (via EF Core)
```

The real JWT never leaves the httpOnly cookie — it is attached to backend requests
only on the server side and is inaccessible to client-side JavaScript.

---

## Design Decisions

- **`CreatedById` is resolved from JWT claims**, not client input — controllers extract `ClaimTypes.NameIdentifier` from the authenticated token so clients cannot spoof creator identity.
- **Role-based access**: `POST`, `PUT`, and status-changing `PATCH` endpoints require the `RegAffairsOfficer` role; `GET` endpoints are open to both roles. The frontend hides write UI for `Viewer` accounts as a UX convenience only — the backend is the actual enforcement point.
- **Enum serialization**: All enums serialize as strings in API responses (e.g. `"Draft"`, `"NDS"`) to keep responses readable without a lookup table.
- **DTOs only at the boundary**: Controllers map Domain entities to Application DTOs before returning responses — EF navigation properties are never exposed directly.
- **Soft delete only**: `Archive`/`Unarchive` endpoints toggle `IsArchived`; there is no hard delete anywhere in the system, so audit history is always preserved.
- **No service layer (backend)**: Controllers access `PharmaDocsDbContext` directly — an intentional simplification for a portfolio-scoped project.
- **No service layer between UI and API (frontend)**: Route Handlers and Server Components call `apiFetch()` directly; there is no client-side state management library (React Context + `useState` only).
