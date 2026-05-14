# PharmaDocs

A Health Canada–aligned full-stack web application for managing pharmaceutical regulatory submission packages.

Built for Canadian pharmaceutical regulatory affairs teams who currently manage submission workflows in Excel and email threads. PharmaDocs models the document lifecycle and submission workflow from initial draft through Health Canada approval.

---

## Tech Stack

### Backend

- ASP.NET Core 8 Web API
- Entity Framework Core 9
- PostgreSQL
- Clean Architecture (Domain / Application / Infrastructure / API)
- JWT Authentication
- BCrypt password hashing
- FluentValidation

### Frontend

- Next.js 14 (App Router)
- TypeScript
- Tailwind CSS

### DevOps

- Docker / Docker Compose
- Railway (backend + database)
- Vercel (frontend)

---

## Domain Context

PharmaDocs models five Health Canada submission types:

| Code | Full Name                         |
| ---- | --------------------------------- |
| NDS  | New Drug Submission               |
| ANDS | Abbreviated New Drug Submission   |
| SNDS | Supplement to New Drug Submission |
| DMF  | Drug Master File                  |
| CTA  | Clinical Trial Application        |

Documents are tracked through a lifecycle: `Draft → UnderReview → Approved / Rejected`.  
Submission packages are tracked through: `Draft → Submitted → UnderReview → Approved / Rejected / Withdrawn`.

---

## Project Structure

```
pharma-docs/
├── backend/
│   ├── PharmaDocs.API/             ← ASP.NET Core Web API (controllers, middleware)
│   ├── PharmaDocs.Application/     ← Use cases, DTOs, service interfaces
│   ├── PharmaDocs.Domain/          ← Entities, enums, domain logic
│   └── PharmaDocs.Infrastructure/  ← EF Core DbContext, repositories, JWT
├── frontend/
│   └── pharma-docs-web/            ← Next.js application
│       ├── app/                    ← App Router pages
│       ├── components/             ← Shared UI components
│       └── lib/                    ← API client, auth utilities
├── docker-compose.yml
└── README.md
```

---

## Getting Started

### Prerequisites

- .NET 8 SDK
- PostgreSQL
- Node.js 18+

### Backend

```bash
cd backend
cp PharmaDocs.API/appsettings.example.json PharmaDocs.API/appsettings.json
# Fill in your PostgreSQL connection string and JWT secret
dotnet ef database update --project PharmaDocs.Infrastructure --startup-project PharmaDocs.API
dotnet run --project PharmaDocs.API
```

API runs at `https://localhost:5001`  
Swagger UI at `https://localhost:5001/swagger`

### Frontend

```bash
cd frontend/pharma-docs-web
npm install
npm run dev
```

Frontend runs at `http://localhost:3000`

---

## Security

- Passwords hashed with BCrypt (work factor 12)
- JWT tokens with configurable expiry
- Role-based access control (RegAffairsOfficer / Viewer)
- Full audit logging on all submission status changes
- HTTPS enforced in production

---

## Status

🚧 Active development — backend in progress.

---

## Author

Medhat Elsayed — [GitHub](https://github.com/des4mir)
