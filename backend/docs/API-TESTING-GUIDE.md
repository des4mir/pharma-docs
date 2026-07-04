# PharmaDocs API Testing Guide

**Base URL:** `http://localhost:5046`  
**Version:** 0.10.0  
**Last updated:** 2026-07-04

---

## Seeded Test Data

### Users

| Email               | Password  | Role              |
| ------------------- | --------- | ----------------- |
| sarah@pharmadocs.ca | Demo1234! | RegAffairsOfficer |
| james@pharmadocs.ca | Demo1234! | Viewer            |

### Seeded GUIDs

| Entity                                    | ID                                     |
| ----------------------------------------- | -------------------------------------- |
| Product — Atorvastatin 20mg               | `aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa` |
| Product — Metformin 500mg                 | `bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb` |
| Document — Atorvastatin Product Monograph | `cccccccc-cccc-cccc-cccc-cccccccccccc` |
| Submission Package — NDS Atorvastatin     | `eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee` |

---

## Endpoints Reference

### Auth

| Method | Route             | Auth | Notes             |
| ------ | ----------------- | ---- | ----------------- |
| POST   | `/api/auth/login` | None | Returns JWT token |

**Request body:**

```json
{ "email": "sarah@pharmadocs.ca", "password": "Demo1234!" }
```

**Success response (200):**

```json
{
  "token": "<jwt>",
  "fullName": "Sarah Leblanc",
  "email": "sarah@pharmadocs.ca",
  "role": "RegAffairsOfficer",
  "expiresAt": "..."
}
```

---

### Products

| Method | Route                          | Auth     | Role              |
| ------ | ------------------------------ | -------- | ----------------- |
| GET    | `/api/products`                | Required | Both              |
| GET    | `/api/products/{id}`           | Required | Both              |
| POST   | `/api/products`                | Required | RegAffairsOfficer |
| PUT    | `/api/products/{id}`           | Required | RegAffairsOfficer |
| PATCH  | `/api/products/{id}/archive`   | Required | RegAffairsOfficer |
| PATCH  | `/api/products/{id}/unarchive` | Required | RegAffairsOfficer |

- `GET` returns only non-archived products, ordered by name
- `GET /{id}` returns `404` if the product does not exist or is archived
- Archive returns `204 No Content`; returns `404` if already archived
- Unarchive returns `204 No Content`; returns `404` if the product does not exist or is not archived

---

### Documents

| Method | Route                                   | Auth     | Role              |
| ------ | --------------------------------------- | -------- | ----------------- |
| GET    | `/api/documents`                        | Required | Both              |
| GET    | `/api/documents/{id}`                   | Required | Both              |
| GET    | `/api/documents/by-product/{productId}` | Required | Both              |
| POST   | `/api/documents`                        | Required | RegAffairsOfficer |
| PUT    | `/api/documents/{id}`                   | Required | RegAffairsOfficer |
| PATCH  | `/api/documents/{id}/archive`           | Required | RegAffairsOfficer |
| PATCH  | `/api/documents/{id}/unarchive`         | Required | RegAffairsOfficer |

- `GET` variants return only non-archived documents
- `POST` body must include `status` field (e.g. `"Draft"`)
- Archive returns `204 No Content`; returns `404` if already archived
- Unarchive returns `204 No Content`; returns `404` if the document does not exist or is not archived

---

### Submission Packages

| Method | Route                                            | Auth     | Role              |
| ------ | ------------------------------------------------ | -------- | ----------------- |
| GET    | `/api/submissionpackages`                        | Required | Both              |
| GET    | `/api/submissionpackages/{id}`                   | Required | Both              |
| GET    | `/api/submissionpackages/by-product/{productId}` | Required | Both              |
| POST   | `/api/submissionpackages`                        | Required | RegAffairsOfficer |
| PUT    | `/api/submissionpackages/{id}`                   | Required | RegAffairsOfficer |
| PATCH  | `/api/submissionpackages/{id}/status`            | Required | RegAffairsOfficer |
| PATCH  | `/api/submissionpackages/{id}/archive`           | Required | RegAffairsOfficer |
| PATCH  | `/api/submissionpackages/{id}/unarchive`         | Required | RegAffairsOfficer |

- `GET` variants return only non-archived packages
- `PATCH /status` updates status and writes an audit log entry; returns updated package (`200`)
- `PATCH /archive` is blocked if status is `Submitted` or `UnderReview` — returns `400 Bad Request`
- `PATCH /unarchive` returns `204 No Content`; returns `404` if the package does not exist or is not archived

---

### Audit Logs

| Method | Route                                            | Auth     | Role |
| ------ | ------------------------------------------------ | -------- | ---- |
| GET    | `/api/auditlogs?entityType={type}`               | Required | Both |
| GET    | `/api/auditlogs?entityType={type}&entityId={id}` | Required | Both |

- `entityType` is **required**. Valid values: `Product`, `DocumentRecord`, `SubmissionPackage`
- Omitting `entityType` or passing an invalid value returns `400 Bad Request`
- `entityId` is optional — narrows results to a specific entity within the given type
- Unknown `entityId` with a valid `entityType` returns `200 OK` with an empty array `[]`
- Results ordered by `timestamp` descending

**Example calls:**

```
GET /api/auditlogs?entityType=Product
GET /api/auditlogs?entityType=Product&entityId=aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa
GET /api/auditlogs?entityType=DocumentRecord
GET /api/auditlogs?entityType=DocumentRecord&entityId=cccccccc-cccc-cccc-cccc-cccccccccccc
GET /api/auditlogs?entityType=SubmissionPackage
GET /api/auditlogs?entityType=SubmissionPackage&entityId=eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee
```

---

## Role-Based Access Summary

| Action                                | RegAffairsOfficer | Viewer |
| ------------------------------------- | ----------------- | ------ |
| `POST /api/auth/login`                | ✅                | ✅     |
| `GET` any resource                    | ✅                | ✅     |
| `GET /api/auditlogs`                  | ✅                | ✅     |
| `POST` / `PUT` / `PATCH` any resource | ✅                | ❌ 403 |
| `PATCH /{id}/archive`                 | ✅                | ❌ 403 |
| `PATCH /{id}/unarchive`               | ✅                | ❌ 403 |

---

## Expected Test Results

### Step 1 — Login

| Scenario                        | Expected           |
| ------------------------------- | ------------------ |
| sarah@pharmadocs.ca / Demo1234! | 200 OK + JWT token |
| james@pharmadocs.ca / Demo1234! | 200 OK + JWT token |
| Wrong credentials               | 401 Unauthorized   |

### Step 1.5 — Setup Baseline State

The smoke test calls `PATCH /unarchive` as Sarah for the seeded product, document, and submission package before read/write checks begin.

| Scenario                        | Expected       |
| ------------------------------- | -------------- |
| Seeded product unarchive setup  | `204` or `404` |
| Seeded document unarchive setup | `204` or `404` |
| Seeded submission package setup | `204` or `404` |

`204` means the seeded entity was archived and has been restored. `404` is acceptable during setup because the entity may already be active; the API returns `404` when an unarchive target is missing or not currently archived.

### Step 2 — Products: reads

- GET all (officer) → `200 OK`, array includes Atorvastatin 20mg and Metformin 500mg with `createdByName`
- GET all (viewer) → `200 OK`
- GET all (unauthenticated) → `401`
- GET `/{id}` (officer) → `200 OK` with `createdByName`
- GET `/{id}` (viewer) → `200 OK`
- GET unknown GUID → `404`

### Step 3 — Products: writes

- POST (officer) → `201 Created`
- POST (viewer) → `403 Forbidden`
- PUT (officer) → `200 OK`
- PUT (viewer) → `403 Forbidden`
- PATCH `/{id}/archive` (viewer) → `403 Forbidden`
- PATCH `/{id}/archive` (officer) → `204 No Content`
- PATCH `/{id}/archive` again (already archived) → `404`

### Step 4 — Documents: reads

- GET all (officer) → `200 OK`, includes `createdByName`
- GET all (viewer) → `200 OK`
- GET all (unauthenticated) → `401`
- GET `/by-product/{productId}` (officer) → `200 OK`
- GET `/by-product/{productId}` (viewer) → `200 OK`
- GET `/{id}` (officer) → `200 OK`

### Step 5 — Documents: writes

- POST (officer) → `201 Created` (must include `status` in body)
- POST (viewer) → `403 Forbidden`
- PUT (officer) → `200 OK`
- PUT (viewer) → `403 Forbidden`
- PATCH `/{id}/archive` (viewer) → `403 Forbidden`
- PATCH `/{id}/archive` (officer) → `204 No Content`

### Step 6 — Submission Packages: reads

- GET all (officer) → `200 OK`
- GET all (viewer) → `200 OK`
- GET all (unauthenticated) → `401`
- GET `/{id}` (officer) → `200 OK`
- GET `/{id}` (viewer) → `200 OK`
- GET `/by-product/{productId}` (officer) → `200 OK`

### Step 7 — Submission Packages: writes

- POST (officer) → `201 Created`
- POST (viewer) → `403 Forbidden`
- PUT (officer) → `200 OK`
- PUT (viewer) → `403 Forbidden`
- PATCH `/{id}/status` (officer) → `200 OK` with updated package body
- PATCH `/{id}/status` (viewer) → `403 Forbidden`
- PATCH `/{id}/archive` (viewer) → `403 Forbidden`
- PATCH `/{id}/archive` (officer, package is `InProgress`) → `204 No Content`

### Step 8 — Audit Logs

- GET `?entityType=Product` (officer) → `200 OK`
- GET `?entityType=Product` (viewer) → `200 OK`
- GET `?entityType=Product` (unauthenticated) → `401`
- GET `?entityType=DocumentRecord` (officer) → `200 OK`
- GET `?entityType=SubmissionPackage` (officer) → `200 OK`
- GET `?entityType=Product&entityId={id}` → `200 OK`, entries scoped to that product
- GET `?entityType=DocumentRecord&entityId={id}` → `200 OK`
- GET `?entityType=SubmissionPackage&entityId={id}` → `200 OK`
- GET `?entityType=Product&entityId=99999999-...` (unknown) → `200 OK`, empty array `[]`
- GET `?entityType=Invalid` → `400 Bad Request`
- GET with no `entityType` → `400 Bad Request`

### Step 9 — Cleanup: unarchive seeded entries

The smoke test archives the seeded product, document, and submission package during earlier write checks, then restores them here and verifies role enforcement.

- PATCH `/{id}/unarchive` (viewer) on each seeded entity → `403 Forbidden`
- PATCH `/{id}/unarchive` (officer) on each seeded entity → `204 No Content`

---

## Troubleshooting

| Symptom                                     | Likely Cause                                                                         |
| ------------------------------------------- | ------------------------------------------------------------------------------------ |
| `401` on all requests                       | Token missing or expired in `Authorization: Bearer` header                           |
| `403` on write operations                   | Logged in as Viewer — use sarah@pharmadocs.ca                                        |
| `404` on product, document, or package read | Resource is archived or GUID is wrong                                                |
| `404` on `PATCH /unarchive`                 | Resource does not exist or is already active / not archived                          |
| Step 1.5 setup returns `404`                | Acceptable baseline behavior; the seeded entity was already active before the test   |
| `400` on audit log request                  | `entityType` missing or not one of: `Product`, `DocumentRecord`, `SubmissionPackage` |
| `400` on submission package archive         | Status is `Submitted` or `UnderReview`                                               |
| Empty `[]` from audit log                   | Valid — no audit entries exist yet for that entity                                   |
