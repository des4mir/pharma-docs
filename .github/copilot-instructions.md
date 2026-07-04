# GitHub Copilot Workspace Instructions

# PharmaDocs — Health Canada–Aligned Pharmaceutical Regulatory Document Manager

## Project Overview

PharmaDocs is a full-stack web application that models the document tracking and
submission lifecycle management workflow used by Canadian pharmaceutical companies
filing regulatory submissions to Health Canada. It is a portfolio project built to
demonstrate Clean Architecture on the backend and Next.js App Router on the frontend.

The backend is **feature-complete** as of v0.10.0. Active work is on the **frontend**.

---

## Repository Structure

```
pharma-docs/
├── .github/
│   └── copilot-instructions.md   ← THIS FILE
├── backend/
│   ├── PharmaDocs.API/            ← ASP.NET Core 8 Web API (controllers, middleware, Program.cs)
│   │   └── Controllers/
│   │       ├── AuthController.cs
│   │       ├── ProductsController.cs
│   │       ├── DocumentsController.cs
│   │       ├── SubmissionPackagesController.cs
│   │       └── AuditLogsController.cs
│   ├── docs/
│   │   ├── API-TESTING-GUIDE.md
│   │   ├── curl-commands.txt
│   │   └── test-api.ps1
│   ├── PharmaDocs.Application/    ← DTOs only (no services layer yet)
│   │   └── DTOs/
│   ├── PharmaDocs.Domain/         ← Entities and Enums (no business logic methods)
│   │   ├── Entities/
│   │   └── Enums/
│   └── PharmaDocs.Infrastructure/ ← EF Core DbContext, migrations, JWT service
│       └── Data/
│           └── PharmaDocsDbContext.cs
├── frontend/
│   └── pharma-docs-web/           ← Next.js 14 App Router application (in progress)
│       ├── app/
│       ├── components/
│       └── lib/
├── CHANGELOG.md
├── docker-compose.yml
└── README.md
```

---

## Tech Stack

### Backend (complete)

- **Language**: C# 12
- **Framework**: ASP.NET Core 8 Web API
- **ORM**: Entity Framework Core 8, code-first migrations
- **Database**: PostgreSQL (local dev via docker-compose, production on Railway)
- **Auth**: JWT Bearer tokens + BCrypt password hashing
- **Architecture**: Clean Architecture — Domain / Application / Infrastructure / API layers
- **Base URL (local)**: `http://localhost:5046`

### Frontend (in progress)

- **Framework**: Next.js 14, App Router
- **Language**: TypeScript (strict mode)
- **Styling**: Tailwind CSS
- **State**: React Context + useState (no Redux, no Zustand)
- **HTTP**: native `fetch` with a typed wrapper in `lib/api.ts`
- **Auth**: JWT stored in `httpOnly` cookie via Next.js Route Handler — never localStorage

---

## Domain Language

Use these terms consistently. Do not invent synonyms.

| Term                    | Meaning                                                                                            |
| ----------------------- | -------------------------------------------------------------------------------------------------- |
| **Product**             | A pharmaceutical product with a DIN or NPN registered in Canada                                    |
| **DocumentRecord**      | A regulatory document attached to a Product (entity name in C#); displayed as "Document" in the UI |
| **SubmissionPackage**   | A collection of documents assembled for a Health Canada submission                                 |
| **AuditLog**            | An immutable record of every create, update, archive, unarchive, or status change                  |
| **RegAffairsOfficer**   | The write role — can create, update, archive, unarchive, and change status                         |
| **Viewer**              | The read-only role — can only call GET endpoints                                                   |
| **Archive / Unarchive** | Soft delete / restore. Never use "delete" — there is no hard delete in this system                 |
| **InProgress**          | SubmissionStatus value between Draft and Submitted                                                 |
| **eCTD**                | Electronic Common Technical Document — the Health Canada submission format this app models         |

---

## Backend Conventions

### Controllers

- All controllers inherit `ControllerBase`, decorated with `[ApiController]`, `[Authorize]`, `[Produces("application/json")]`
- Routes follow the pattern `api/[controller]` (lowercase plural noun)
- `AuditLogsController` is the exception — its route is explicitly `api/auditlogs`
- Every mutating action (POST, PUT, PATCH) writes an `AuditLog` entry before `SaveChangesAsync()`
- Actor identity is always extracted from JWT claims: `User.FindFirstValue(ClaimTypes.NameIdentifier)` for ID, `User.FindFirstValue(ClaimTypes.Name)` for name
- `Archive` returns `204 No Content`; returns `404` if already archived
- `Unarchive` returns `204 No Content`; returns `404` if not currently archived
- `SubmissionPackage.Archive` returns `400 Bad Request` if status is `Submitted` or `UnderReview`

### Response Codes

| Scenario                      | Code                                 |
| ----------------------------- | ------------------------------------ |
| Successful read               | 200 OK                               |
| Resource created              | 201 Created (with `CreatedAtAction`) |
| Archive / Unarchive success   | 204 No Content                       |
| Viewer attempts write         | 403 Forbidden                        |
| No token                      | 401 Unauthorized                     |
| Not found or already archived | 404 Not Found                        |
| Business rule violation       | 400 Bad Request                      |

### DTOs (in PharmaDocs.Application/DTOs/)

- `Create*Dto` — input for POST
- `Update*Dto` — input for PUT
- `*ResponseDto` — output for all reads and write responses
- Never expose `PasswordHash`, internal user IDs in audit trail output, or EF navigation objects directly
- `DocumentResponseDto` and `ProductResponseDto` include `createdByName` (denormalized string)

### Entities (in PharmaDocs.Domain/Entities/)

- All entities use `Guid` PKs
- All entities have `CreatedAt` (DateTime UTC) and `CreatedById` (FK to User)
- Soft-deletable entities (`Product`, `DocumentRecord`, `SubmissionPackage`) have:
  `IsArchived`, `ArchivedAt?`, `ArchivedById?`, navigation `ArchivedBy?`
- Enums are stored as strings in PostgreSQL via `.HasConversion<string>()` in `OnModelCreating`

### Enums

```
UserRole:          RegAffairsOfficer | Viewer
DocumentType:      CertificateOfAnalysis | ProductMonograph | BatchRecord |
                   TemperatureStorageLog | ProductSpecificationSheet |
                   ImportClearance | SubmissionCertificate |
                   ClinicalStudyReport | NonClinicalStudyReport | QualitySummary
DocumentStatus:    Draft | Final | Superseded
SubmissionType:    NDS | ANDS | SNDS | DMF
SubmissionStatus:  Draft | InProgress | Submitted | UnderReview |
                   Approved | Rejected | Withdrawn
```

### AuditLog Action Values

Use exactly these strings for the `Action` field — no variations:
`"Created"` | `"Updated"` | `"Archived"` | `"Unarchived"` | `"StatusChanged"`

### Seeded GUIDs (for tests and dev)

```
User — Sarah (RegAffairsOfficer):     11111111-1111-1111-1111-111111111111
User — James (Viewer):                22222222-2222-2222-2222-222222222222
Product — Atorvastatin 20mg:          aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa
Product — Metformin 500mg:            bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb
Document — Atorvastatin Monograph:    cccccccc-cccc-cccc-cccc-cccccccccccc
Document — Metformin CoA:             dddddddd-dddd-dddd-dddd-dddddddddddd
SubmissionPackage — NDS Atorvastatin: eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee
Seed password (both users):           Demo1234!
```

### API Endpoints (current — v0.10.0)

```
POST   /api/auth/login

GET    /api/products
GET    /api/products/{id}
POST   /api/products
PUT    /api/products/{id}
PATCH  /api/products/{id}/archive
PATCH  /api/products/{id}/unarchive

GET    /api/documents
GET    /api/documents/{id}
GET    /api/documents/by-product/{productId}
POST   /api/documents
PUT    /api/documents/{id}
PATCH  /api/documents/{id}/archive
PATCH  /api/documents/{id}/unarchive

GET    /api/submissionpackages
GET    /api/submissionpackages/{id}
GET    /api/submissionpackages/by-product/{productId}
POST   /api/submissionpackages
PUT    /api/submissionpackages/{id}
PATCH  /api/submissionpackages/{id}/status
PATCH  /api/submissionpackages/{id}/archive
PATCH  /api/submissionpackages/{id}/unarchive

GET    /api/auditlogs?entityType={type}[&entityId={guid}]
       entityType: Product | DocumentRecord | SubmissionPackage (required)
       entityId: guid (optional) — omit to get all entries for the type
```

---

## Frontend Conventions

### File & Folder Rules

- All pages live under `frontend/pharma-docs-web/app/`
- Shared components live under `frontend/pharma-docs-web/components/`
- API client and auth utilities live under `frontend/pharma-docs-web/lib/`
- Use kebab-case for folders, PascalCase for component files (e.g. `ProductCard.tsx`)
- Every page is a React Server Component by default; add `"use client"` only when hooks or browser APIs are needed

### API Client (`lib/api.ts`)

- All API calls go through a single typed `apiFetch(path, options)` wrapper
- The wrapper reads `NEXT_PUBLIC_API_URL` for the base URL
- The JWT is attached from the `httpOnly` cookie on the server side; the client side never reads it directly
- All response types mirror the backend `*ResponseDto` shapes — define TypeScript interfaces for each

### Auth

- Login flow: form → `POST /app/api/auth/login` (Next.js Route Handler) → sets `httpOnly` cookie → redirects to `/`
- `middleware.ts` at the project root protects all routes except `/login` — redirect to `/login` if no valid cookie
- `AuthContext` exposes `{ user: { fullName, email, role } | null, logout }`
- Role checks in the UI: hide write actions (buttons, forms) when `role !== "RegAffairsOfficer"` — but always rely on the backend for actual enforcement

### TypeScript Types

Define types in `lib/types.ts`. Mirror backend response shapes exactly:

```typescript
// Core types (expand as needed)
type UserRole = "RegAffairsOfficer" | "Viewer";
type SubmissionStatus =
  | "Draft"
  | "InProgress"
  | "Submitted"
  | "UnderReview"
  | "Approved"
  | "Rejected"
  | "Withdrawn";
type DocumentStatus = "Draft" | "Final" | "Superseded";
type DocumentType =
  | "CertificateOfAnalysis"
  | "ProductMonograph"
  | "BatchRecord"
  | "TemperatureStorageLog"
  | "ProductSpecificationSheet"
  | "ImportClearance"
  | "SubmissionCertificate"
  | "ClinicalStudyReport"
  | "NonClinicalStudyReport"
  | "QualitySummary";
type SubmissionType = "NDS" | "ANDS" | "SNDS" | "DMF";
type AuditAction =
  | "Created"
  | "Updated"
  | "Archived"
  | "Unarchived"
  | "StatusChanged";
```

### Styling

- Tailwind CSS only — no inline styles, no CSS modules, no styled-components
- Use semantic HTML: `<main>`, `<nav>`, `<header>`, `<section>`, `<form>`, `<label>`
- All form inputs must have an associated `<label>`
- Status badges for `SubmissionStatus` use distinct colors:
  - Draft → gray, InProgress → blue, Submitted → yellow, UnderReview → orange,
    Approved → green, Rejected → red, Withdrawn → slate

### Component Patterns

- `<AuditLogDrawer entityType="..." entityId="..." />` — reusable audit log panel,
  fetches `GET /api/auditlogs?entityType=...&entityId=...` and renders a timestamp-descending timeline
- `<StatusBadge status={...} />` — renders a colored pill for any SubmissionStatus value
- `<ArchiveButton entityType="..." entityId="..." isArchived={...} />` — handles archive/unarchive
  toggle, only renders for `RegAffairsOfficer` role

---

## What NOT to Generate

- Do not add a hard delete (`DELETE`) endpoint to any controller — this system uses soft delete only
- Do not add a `register` endpoint — user management is out of scope for MVP (users are seeded)
- Do not use `localStorage` for the JWT token
- Do not add a service layer between controllers and DbContext — the current architecture accesses `PharmaDocsDbContext` directly from controllers, intentionally kept simple for a portfolio project
- Do not add Redux, Zustand, or any external state management library — use React Context only
- Do not add file upload functionality — file storage is a Phase 2 feature
- Do not change the `AuditLog` table schema — audit records are immutable once written
- Do not store enum values as integers — all enums use `.HasConversion<string>()`

---

## Local Development

### Backend

```bash
cd backend
dotnet run --project PharmaDocs.API
# API: http://localhost:5046
# Swagger: http://localhost:5046/swagger
```

### Database (PostgreSQL via Docker)

```bash
docker-compose up -d   # starts PostgreSQL
cd backend
dotnet ef database update --project PharmaDocs.Infrastructure --startup-project PharmaDocs.API
```

### Frontend

```bash
cd frontend/pharma-docs-web
npm install
npm run dev
# App: http://localhost:3000
```

### Environment Variables

Backend (`backend/PharmaDocs.API/appsettings.Development.json`):

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=pharmadocs;Username=postgres;Password=postgres"
  },
  "Jwt": { "Key": "<secret>", "Issuer": "PharmaDocs", "Audience": "PharmaDocs" }
}
```

Frontend (`.env.local`):

```
NEXT_PUBLIC_API_URL=http://localhost:5046
JWT_SECRET=<same secret as backend>
```

---

## Current Status (as of 2026-06-28)

Backend complete ✅ — all controllers, DTOs, entities, migrations, seed data, and testing files done.

Frontend in progress 🔄 — Next.js project structure to be initialized. See blueprint milestones for ordered task list.

Next task: Initialize Next.js project, set up `lib/api.ts` and `lib/types.ts`, build the auth layer (`/login` page + middleware + AuthContext).
