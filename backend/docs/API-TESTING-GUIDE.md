# PharmaDocs API - Testing Guide

## Quick Start

### 1. Start the API

```powershell
cd PharmaDocs.API
dotnet run
# API will be available at: http://localhost:5046
```

### 2. Determine the Seeded Password

The database contains two test users with the same password hash:

- **Hash**: `$2a$11$gC76SgMbnnNGOHdy6WKR/uaAL2ZkBonnlNbdr5M/bLYCb8C1NTGBu`

- **Test passwords: `Demo1234!`**

### 3. Run the Test Script

```powershell
# Navigate to backend directory
cd d:\OneDrive\code\pharma-docs\backend

# Run the PowerShell test script
.\test-api.ps1
```

## Test Users

### User 1: RegAffairsOfficer (Has write permissions)

- **Email**: `sarah@pharmadocs.ca`
- **Full Name**: Sarah Leblanc
- **ID**: `11111111-1111-1111-1111-111111111111`
- **Role**: RegAffairsOfficer
- **Permissions**: Can READ, CREATE, UPDATE, DELETE products and documents

### User 2: Viewer (Read-only)

- **Email**: `james@pharmadocs.ca`
- **Full Name**: James Okafor
- **ID**: `22222222-2222-2222-2222-222222222222`
- **Role**: Viewer
- **Permissions**: Can only READ products and documents (no create/update/delete)

## API Endpoints

### Authentication

- `POST /api/auth/login` - Login and get JWT token
  - **Access**: Public (no auth required)
  - **Required fields**: `email`, `password`
  - **Returns**: Token, user info, expiration time

### Products (All require authentication)

- `GET /api/products` - List all products
  - **Access**: Both roles
- `GET /api/products/{id}` - Get single product
  - **Access**: Both roles
- `POST /api/products` - Create product
  - **Access**: RegAffairsOfficer only (403 Forbidden for Viewer)
- `PUT /api/products/{id}` - Update product
  - **Access**: RegAffairsOfficer only (403 Forbidden for Viewer)
- `DELETE /api/products/{id}` - Delete product
  - **Access**: RegAffairsOfficer only (403 Forbidden for Viewer)

### Documents (All require authentication)

- `GET /api/documents` - List all documents
  - **Access**: Both roles
- `GET /api/documents/by-product/{productId}` - Get documents for a product
  - **Access**: Both roles
- `GET /api/documents/{id}` - Get single document
  - **Access**: Both roles
- `POST /api/documents` - Create document
  - **Access**: RegAffairsOfficer only (403 Forbidden for Viewer)
- `PUT /api/documents/{id}` - Update document
  - **Access**: RegAffairsOfficer only (403 Forbidden for Viewer)
- `DELETE /api/documents/{id}` - Delete document
  - **Access**: RegAffairsOfficer only (403 Forbidden for Viewer)

## Testing Methods

### Option 1: PowerShell Script (Recommended)

```powershell
.\test-api.ps1
```

- Automated testing of all endpoints
- Tests both user roles
- Shows success/failure for each endpoint
- Tests role-based access control
- Tests unauthenticated access

### Option 2: curl Commands

See `curl-commands.txt` for manual curl commands.

Example login:

```bash
curl -X POST http://localhost:5046/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"sarah@pharmadocs.ca","password":"PASSWORD_HERE"}'
```

Then save the token and use it for subsequent requests:

```bash
curl -X GET http://localhost:5046/api/products \
  -H "Authorization: Bearer YOUR_TOKEN_HERE"
```

### Option 3: Swagger UI

Visit: http://localhost:5046/swagger

- Interactive API documentation
- Can test endpoints directly from browser
- Click "Authorize" button to add JWT token

## Seeded Data

### Products

1. **Atorvastatin 20mg Tablet**
   - ID: `aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa`
   - DIN: 02245276
   - Manufacturer: Apotex Inc.

2. **Metformin 500mg Tablet**
   - ID: `bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb`
   - DIN: 02162512
   - Manufacturer: Teva Canada

### Documents

1. **Atorvastatin Product Monograph v1.0**
   - ID: `cccccccc-cccc-cccc-cccc-cccccccccccc`
   - Status: Final
   - Type: ProductMonograph

2. **Metformin Label Draft**
   - ID: `dddddddd-dddd-dddd-dddd-dddddddddddd`
   - Status: Draft
   - Type: Label

## Expected Test Results

### RegAffairsOfficer (sarah@pharmadocs.ca)

```
✓ Login: 200 OK
✓ GET /api/products: 200 OK
✓ GET /api/products/{id}: 200 OK
✓ POST /api/products: 201 Created
✓ PUT /api/products/{id}: 200 OK
✓ DELETE /api/products/{id}: 200 OK (or 204 No Content)
✓ GET /api/documents: 200 OK
✓ GET /api/documents/by-product/{id}: 200 OK
✓ GET /api/documents/{id}: 200 OK
✓ POST /api/documents: 201 Created
✓ PUT /api/documents/{id}: 200 OK
✓ DELETE /api/documents/{id}: 200 OK (or 204 No Content)
```

### Viewer (james@pharmadocs.ca)

```
✓ Login: 200 OK
✓ GET /api/products: 200 OK
✓ GET /api/products/{id}: 200 OK
✗ POST /api/products: 403 Forbidden
✗ PUT /api/products/{id}: 403 Forbidden
✗ DELETE /api/products/{id}: 403 Forbidden
✓ GET /api/documents: 200 OK
✓ GET /api/documents/by-product/{id}: 200 OK
✓ GET /api/documents/{id}: 200 OK
✗ POST /api/documents: 403 Forbidden
✗ PUT /api/documents/{id}: 403 Forbidden
✗ DELETE /api/documents/{id}: 403 Forbidden
```

### Unauthenticated

```
✗ GET /api/products: 401 Unauthorized
✗ GET /api/documents: 401 Unauthorized
```

## Troubleshooting

### Login fails with "Invalid email or password"

- Double-check the password you're using
- Ensure the user email is exactly: `sarah@pharmadocs.ca` or `james@pharmadocs.ca`
- The password hash suggests it might be a common test password

### 401 Unauthorized on authenticated endpoints

- Ensure you're including the Authorization header: `Authorization: Bearer YOUR_TOKEN_HERE`
- Ensure the token hasn't expired
- Ensure the token is from the login response, not the user ID

### 403 Forbidden on POST/PUT/DELETE endpoints

- This is expected if you're using the "Viewer" role
- Use the "RegAffairsOfficer" (sarah@pharmadocs.ca) account to test write operations
- Different roles have different permissions by design

### Connection refused

- Ensure the API is running: `cd PharmaDocs.API && dotnet run`
- Check that the API is listening on port 5046
- The launchSettings.json shows it should run on http://localhost:5046

## Password Reset (if needed)

If you forget the password, you can reset it in the database:

1. Open `PharmaDocsDbContext.cs`
2. Find the `SeedPasswordHash` constant
3. Generate a new BCrypt hash using:
   ```csharp
   var hashedPassword = BCrypt.Net.BCrypt.HashPassword("YourNewPassword");
   ```
4. Update both the seeding constant and the migration files
5. Run: `dotnet ef database update`
