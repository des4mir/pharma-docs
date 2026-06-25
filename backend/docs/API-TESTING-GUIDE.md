# PharmaDocs API Testing Guide

**Base URL:** `http://localhost:5046`  
**Version:** 0.9.0  
**Last updated:** 2026-06-24

---

## Seeded Test Data

### Users

| Email               | Password  | Role              |
| ------------------- | --------- | ----------------- |
| sarah@pharmadocs.ca | Demo1234! | RegAffairsOfficer |
| james@pharmadocs.ca | Demo1234! | Viewer            |

### Seeded GUIDs

| Entity                                | ID                                     |
| ------------------------------------- | -------------------------------------- |
| Product — Atorvastatin 20mg           | `aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa` |
| Product — Metformin 500mg             | `bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb` |
| Document — Atorvastatin Product Monograph | `cccccccc-cccc-cccc-cccc-cccccccccccc` |
| Submission Package — NDS Atorvastatin | `eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee` |

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

| Method | Route                        | Auth     | Role              |
| ------ | ---------------------------- | -------- | ----------------- |
| GET    | `/api/products`              | Required | Both              |
| GET    | `/api/products/{id}`         | Required | Both              |
| POST   | `/api/products`              | Required | RegAffairsOfficer |
| PUT    | `/api/products/{id}`         | Required | RegAffairsOfficer |
| PATCH  | `/api/products/{id}/archive` | Required | RegAffairsOfficer |

- `GET` returns only non-archived products, ordered by name
- `GET /{id}` returns `404` if the product does not exist or is archived
- Archive returns `204 No Content`; returns `404` if already archived

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

- `GET` variants return only non-archived documents
- `POST` body must include `status` field (e.g. `"Draft"`)
- Archive returns `204 No Content`; returns `404` if already archived

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

- `GET` variants return only non-archived packages
- `PATCH /status` updates status and writes an audit log entry; returns updated package (`200`)
- `PATCH /archive` is blocked if status is `Submitted` or `UnderReview` — returns `400 Bad Request`

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

---

## Expected Test Results

### Step 1 — Login

| Scenario                        | Expected           |
| ------------------------------- | ------------------ |
| sarah@pharmadocs.ca / Demo1234! | 200 OK + JWT token |
| james@pharmadocs.ca / Demo1234! | 200 OK + JWT token |
| Wrong credentials               | 401 Unauthorized   |

### Step 2 — GET Products

- Both roles → `200 OK`, array containing Atorvastatin 20mg and Metformin 500mg
- Each product includes `createdByName` field
- Unauthenticated → `401`

### Step 3 — GET Single Product

- Both roles → `200 OK` with `createdByName`
- Unknown ID → `404`

### Step 4 — POST Product

- RegAffairsOfficer → `201 Created`
- Viewer → `403 Forbidden`

### Step 5 — PUT Product

- RegAffairsOfficer → `200 OK`
- Viewer → `403 Forbidden`

### Step 6 — PATCH Product Archive

- RegAffairsOfficer → `204 No Content`
- Viewer → `403 Forbidden`
- Archiving already-archived product → `404`

### Step 7 — GET Documents

- Both roles → `200 OK`, includes `createdByName`
- Unauthenticated → `401`

### Step 8 — GET Documents by Product

- Both roles → `200 OK`, scoped to product

### Step 9 — POST Document

- RegAffairsOfficer → `201 Created` (must include `status` in body)
- Viewer → `403 Forbidden`

### Step 10 — PATCH Document Archive

- RegAffairsOfficer → `204 No Content`
- Viewer → `403 Forbidden`

### Step 11 — GET Submission Packages

- Both roles → `200 OK`
- Unauthenticated → `401`

### Step 12 — POST Submission Package

- RegAffairsOfficer → `201 Created`
- Viewer → `403 Forbidden`

### Step 13 — PATCH Submission Package Status

- RegAffairsOfficer → `200 OK` with updated package
- Viewer → `403 Forbidden`

### Step 14 — PATCH Submission Package Archive

- RegAffairsOfficer on `Draft`/`InProgress` package → `204 No Content`
- RegAffairsOfficer on `Submitted`/`UnderReview` package → `400 Bad Request`
- Viewer → `403 Forbidden`

### Step 15 — GET Audit Logs (entityType only)

- Both roles → `200 OK`, array of entries
- Unauthenticated → `401`

### Step 16 — GET Audit Logs (entityType + entityId)

- Both roles → `200 OK`, entries scoped to that entity

### Step 17 — GET Audit Logs (missing or invalid entityType)

- → `400 Bad Request`

### Step 18 — GET Audit Logs (valid entityType, unknown entityId)

- → `200 OK`, empty array `[]`

---

## Troubleshooting

| Symptom                             | Likely Cause                                                                         |
| ----------------------------------- | ------------------------------------------------------------------------------------ |
| `401` on all requests               | Token missing or expired in `Authorization: Bearer` header                           |
| `403` on write operations           | Logged in as Viewer — use sarah@pharmadocs.ca                                        |
| `404` on product or document        | Resource is archived or GUID is wrong                                                |
| `400` on audit log request          | `entityType` missing or not one of: `Product`, `DocumentRecord`, `SubmissionPackage` |
| `400` on submission package archive | Status is `Submitted` or `UnderReview`                                               |
| Empty `[]` from audit log           | Valid — no audit entries exist yet for that entity                                   |
