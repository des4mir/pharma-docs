# Architecture

PharmaDocs uses a four-project Clean Architecture structure.

---

## Layers

| Layer              | Project                     | Responsibility                                                                                                                                                                                                                              |
| ------------------ | --------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| **Domain**         | `PharmaDocs.Domain`         | Core entities (`User`, `Product`, `DocumentRecord`, `SubmissionPackage`, `SubmissionDocument`, `AuditLog`) and enums (`UserRole`, `DocumentType`, `DocumentStatus`, `SubmissionType`, `SubmissionStatus`). No dependencies on other layers. |
| **Application**    | `PharmaDocs.Application`    | API contracts and DTOs: `LoginDto`, `AuthResponseDto`, `CreateProductDto`, `UpdateProductDto`, `ProductResponseDto`, `CreateDocumentDto`, `UpdateDocumentDto`, `DocumentResponseDto`. Depends only on Domain.                               |
| **Infrastructure** | `PharmaDocs.Infrastructure` | `PharmaDocsDbContext`, EF Core / PostgreSQL configuration, migrations, seed data, `JwtTokenService`. Depends on Domain and Application.                                                                                                     |
| **API**            | `PharmaDocs.API`            | ASP.NET Core controllers (`AuthController`, `ProductsController`, `DocumentsController`), middleware pipeline, Swagger config, JWT Bearer setup. Depends on all lower layers.                                                               |

---

## Dependency Flow

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

## Design Decisions

- **`CreatedById` is resolved from JWT claims**, not client input — controllers extract `ClaimTypes.NameIdentifier` from the authenticated token so clients cannot spoof creator identity.
- **Role-based access**: `POST`, `PUT`, `DELETE` endpoints on `ProductsController` and `DocumentsController` require the `RegAffairsOfficer` role; `GET` endpoints are open to both roles.
- **Enum serialization**: All enums serialize as strings in API responses (e.g. `"Draft"`, `"NDS"`) to keep responses readable without a lookup table.
- **DTOs only at the boundary**: Controllers map Domain entities to Application DTOs before returning responses — EF navigation properties are never exposed directly.
