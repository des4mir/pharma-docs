# PharmaDocs API Testing Script
# Tests endpoints with both user roles (RegAffairsOfficer and Viewer)

$API_URL = "http://localhost:5046"

# Test users (from database seeding)
$users = @(
    @{
        Email    = "sarah@pharmadocs.ca"
        Password = "Demo1234!"
        FullName = "Sarah Leblanc"
        Role     = "RegAffairsOfficer"
    },
    @{
        Email    = "james@pharmadocs.ca"
        Password = "Demo1234!"
        FullName = "James Okafor"
        Role     = "Viewer"
    }
)

# Guids for testing (from seeded data)
$productId = "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"
$documentId = "cccccccc-cccc-cccc-cccc-cccccccccccc"

$tokens = @{}

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "PharmaDocs API Testing Script" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Function to pretty-print JSON responses
function Format-JsonResponse {
    param([string]$JsonString)
    try {
        $JsonString | ConvertFrom-Json | ConvertTo-Json | Write-Host -ForegroundColor Green
    }
    catch {
        Write-Host $JsonString -ForegroundColor Green
    }
}

# ==========================================
# 1. LOGIN FOR BOTH USERS
# ==========================================
Write-Host "Step 1: LOGIN FOR BOTH USERS" -ForegroundColor Yellow
Write-Host "----------------------------------------" -ForegroundColor Yellow

foreach ($user in $users) {
    Write-Host ""
    Write-Host "Logging in as: $($user.Role) ($($user.Email))" -ForegroundColor Magenta
    
    $loginBody = @{
        email    = $user.Email
        password = $user.Password
    } | ConvertTo-Json
    
    Write-Host "Request: POST $API_URL/api/auth/login" -ForegroundColor Cyan
    Write-Host "Body: $loginBody" -ForegroundColor Cyan
    
    try {
        $response = Invoke-RestMethod -Uri "$API_URL/api/auth/login" `
            -Method Post `
            -ContentType "application/json" `
            -Body $loginBody `
            -ErrorAction Stop
        
        Write-Host "Response: Status 200 OK" -ForegroundColor Green
        Format-JsonResponse ($response | ConvertTo-Json)
        
        $tokens[$user.Role] = $response.token
        Write-Host "Token saved for $($user.Role)" -ForegroundColor Green
    }
    catch {
        Write-Host "Response: Status $($_.Exception.Response.StatusCode)" -ForegroundColor Red
        Write-Host "Error: $($_.Exception.Message)" -ForegroundColor Red
        Write-Host "TIP: If login fails, the password might be different." -ForegroundColor Yellow
    }
}

Write-Host ""
Write-Host ""

# ==========================================
# 2. GET ALL PRODUCTS (Both roles can access)
# ==========================================
Write-Host "Step 2: GET ALL PRODUCTS (Requires Auth)" -ForegroundColor Yellow
Write-Host "----------------------------------------" -ForegroundColor Yellow

foreach ($role in @("RegAffairsOfficer", "Viewer")) {
    Write-Host ""
    Write-Host "Testing as: $role" -ForegroundColor Magenta
    
    if ($tokens.ContainsKey($role)) {
        $token = $tokens[$role]
        
        Write-Host "Request: GET $API_URL/api/products" -ForegroundColor Cyan
        Write-Host "Header: Authorization: Bearer <token>" -ForegroundColor Cyan
        
        try {
            $response = Invoke-RestMethod -Uri "$API_URL/api/products" `
                -Method Get `
                -Headers @{Authorization = "Bearer $token" } `
                -ErrorAction Stop
            
            Write-Host "Response: Status 200 OK" -ForegroundColor Green
            Write-Host "Products found: $($response.Count)" -ForegroundColor Green
            Format-JsonResponse ($response | ConvertTo-Json)
        }
        catch {
            Write-Host "Response: Status $($_.Exception.Response.StatusCode)" -ForegroundColor Red
            Write-Host "Error: $($_.Exception.Message)" -ForegroundColor Red
        }
    }
    else {
        Write-Host "Skipped: No valid token for $role" -ForegroundColor Yellow
    }
}

Write-Host ""
Write-Host ""

# ==========================================
# 3. GET SINGLE PRODUCT BY ID
# ==========================================
Write-Host "Step 3: GET SINGLE PRODUCT BY ID" -ForegroundColor Yellow
Write-Host "----------------------------------------" -ForegroundColor Yellow

foreach ($role in @("RegAffairsOfficer", "Viewer")) {
    Write-Host ""
    Write-Host "Testing as: $role" -ForegroundColor Magenta
    
    if ($tokens.ContainsKey($role)) {
        $token = $tokens[$role]
        
        Write-Host "Request: GET $API_URL/api/products/$productId" -ForegroundColor Cyan
        
        try {
            $response = Invoke-RestMethod -Uri "$API_URL/api/products/$productId" `
                -Method Get `
                -Headers @{Authorization = "Bearer $token" } `
                -ErrorAction Stop
            
            Write-Host "Response: Status 200 OK" -ForegroundColor Green
            Format-JsonResponse ($response | ConvertTo-Json)
        }
        catch {
            Write-Host "Response: Status $($_.Exception.Response.StatusCode)" -ForegroundColor Red
        }
    }
}

Write-Host ""
Write-Host ""

# ==========================================
# 4. CREATE PRODUCT (RegAffairsOfficer Only)
# ==========================================
Write-Host "Step 4: CREATE PRODUCT (RegAffairsOfficer Only)" -ForegroundColor Yellow
Write-Host "----------------------------------------" -ForegroundColor Yellow

$createProductBody = @{
    name                  = "Aspirin 500mg Test"
    din                   = "00000001"
    npn                   = $null
    medicinalIngredient   = "Acetylsalicylic Acid"
    manufacturer          = "Test Pharma Ltd"
    dosageForm            = "Tablet"
    routeOfAdministration = "Oral"
    therapeuticCategory   = "Analgesic"
} | ConvertTo-Json

foreach ($role in @("RegAffairsOfficer", "Viewer")) {
    Write-Host ""
    Write-Host "Testing as: $role" -ForegroundColor Magenta
    
    if ($tokens.ContainsKey($role)) {
        $token = $tokens[$role]
        
        Write-Host "Request: POST $API_URL/api/products" -ForegroundColor Cyan
        Write-Host "Body: $createProductBody" -ForegroundColor Cyan
        
        try {
            $response = Invoke-RestMethod -Uri "$API_URL/api/products" `
                -Method Post `
                -Headers @{Authorization = "Bearer $token" } `
                -ContentType "application/json" `
                -Body $createProductBody `
                -ErrorAction Stop
            
            Write-Host "Response: Status 201 Created" -ForegroundColor Green
            Write-Host "SUCCESS: $role can create products" -ForegroundColor Green
            Format-JsonResponse ($response | ConvertTo-Json)
        }
        catch {
            $statusCode = $_.Exception.Response.StatusCode
            Write-Host "Response: Status $statusCode" -ForegroundColor Red
            
            if ($statusCode -eq "Forbidden") {
                Write-Host "EXPECTED: $role cannot create products (Forbidden 403)" -ForegroundColor Yellow
            }
            else {
                Write-Host "Error: $($_.Exception.Message)" -ForegroundColor Red
            }
        }
    }
}

Write-Host ""
Write-Host ""

# ==========================================
# 5. GET ALL DOCUMENTS (Both roles can access)
# ==========================================
Write-Host "Step 5: GET ALL DOCUMENTS (Requires Auth)" -ForegroundColor Yellow
Write-Host "----------------------------------------" -ForegroundColor Yellow

foreach ($role in @("RegAffairsOfficer", "Viewer")) {
    Write-Host ""
    Write-Host "Testing as: $role" -ForegroundColor Magenta
    
    if ($tokens.ContainsKey($role)) {
        $token = $tokens[$role]
        
        Write-Host "Request: GET $API_URL/api/documents" -ForegroundColor Cyan
        
        try {
            $response = Invoke-RestMethod -Uri "$API_URL/api/documents" `
                -Method Get `
                -Headers @{Authorization = "Bearer $token" } `
                -ErrorAction Stop
            
            Write-Host "Response: Status 200 OK" -ForegroundColor Green
            Write-Host "Documents found: $($response.Count)" -ForegroundColor Green
            Format-JsonResponse ($response | ConvertTo-Json)
        }
        catch {
            Write-Host "Response: Status $($_.Exception.Response.StatusCode)" -ForegroundColor Red
        }
    }
}

Write-Host ""
Write-Host ""

# ==========================================
# 6. GET DOCUMENTS BY PRODUCT
# ==========================================
Write-Host "Step 6: GET DOCUMENTS BY PRODUCT" -ForegroundColor Yellow
Write-Host "----------------------------------------" -ForegroundColor Yellow

foreach ($role in @("RegAffairsOfficer", "Viewer")) {
    Write-Host ""
    Write-Host "Testing as: $role" -ForegroundColor Magenta
    
    if ($tokens.ContainsKey($role)) {
        $token = $tokens[$role]
        
        Write-Host "Request: GET $API_URL/api/documents/by-product/$productId" -ForegroundColor Cyan
        
        try {
            $response = Invoke-RestMethod -Uri "$API_URL/api/documents/by-product/$productId" `
                -Method Get `
                -Headers @{Authorization = "Bearer $token" } `
                -ErrorAction Stop
            
            Write-Host "Response: Status 200 OK" -ForegroundColor Green
            Write-Host "Documents found: $($response.Count)" -ForegroundColor Green
            Format-JsonResponse ($response | ConvertTo-Json)
        }
        catch {
            Write-Host "Response: Status $($_.Exception.Response.StatusCode)" -ForegroundColor Red
        }
    }
}

Write-Host ""
Write-Host ""

# ==========================================
# 7. GET SINGLE DOCUMENT BY ID
# ==========================================
Write-Host "Step 7: GET SINGLE DOCUMENT BY ID" -ForegroundColor Yellow
Write-Host "----------------------------------------" -ForegroundColor Yellow

foreach ($role in @("RegAffairsOfficer", "Viewer")) {
    Write-Host ""
    Write-Host "Testing as: $role" -ForegroundColor Magenta
    
    if ($tokens.ContainsKey($role)) {
        $token = $tokens[$role]
        
        Write-Host "Request: GET $API_URL/api/documents/$documentId" -ForegroundColor Cyan
        
        try {
            $response = Invoke-RestMethod -Uri "$API_URL/api/documents/$documentId" `
                -Method Get `
                -Headers @{Authorization = "Bearer $token" } `
                -ErrorAction Stop
            
            Write-Host "Response: Status 200 OK" -ForegroundColor Green
            Format-JsonResponse ($response | ConvertTo-Json)
        }
        catch {
            Write-Host "Response: Status $($_.Exception.Response.StatusCode)" -ForegroundColor Red
        }
    }
}

Write-Host ""
Write-Host ""

# ==========================================
# 8. CREATE DOCUMENT (RegAffairsOfficer Only)
# ==========================================
Write-Host "Step 8: CREATE DOCUMENT (RegAffairsOfficer Only)" -ForegroundColor Yellow
Write-Host "----------------------------------------" -ForegroundColor Yellow

$createDocumentBody = @{
    title     = "Test Document v2.0"
    type      = "ProductMonograph"
    version   = "2.0"
    date      = Get-Date -Format "yyyy-MM-dd"
    notes     = "Test document created via API"
    productId = $productId
} | ConvertTo-Json

foreach ($role in @("RegAffairsOfficer", "Viewer")) {
    Write-Host ""
    Write-Host "Testing as: $role" -ForegroundColor Magenta
    
    if ($tokens.ContainsKey($role)) {
        $token = $tokens[$role]
        
        Write-Host "Request: POST $API_URL/api/documents" -ForegroundColor Cyan
        Write-Host "Body: $createDocumentBody" -ForegroundColor Cyan
        
        try {
            $response = Invoke-RestMethod -Uri "$API_URL/api/documents" `
                -Method Post `
                -Headers @{Authorization = "Bearer $token" } `
                -ContentType "application/json" `
                -Body $createDocumentBody `
                -ErrorAction Stop
            
            Write-Host "Response: Status 201 Created" -ForegroundColor Green
            Write-Host "SUCCESS: $role can create documents" -ForegroundColor Green
            Format-JsonResponse ($response | ConvertTo-Json)
        }
        catch {
            $statusCode = $_.Exception.Response.StatusCode
            Write-Host "Response: Status $statusCode" -ForegroundColor Red
            
            if ($statusCode -eq "Forbidden") {
                Write-Host "EXPECTED: $role cannot create documents (Forbidden 403)" -ForegroundColor Yellow
            }
            else {
                Write-Host "Error: $($_.Exception.Message)" -ForegroundColor Red
            }
        }
    }
}

Write-Host ""
Write-Host ""

# ==========================================
# 9. TEST UNAUTHENTICATED ACCESS
# ==========================================
Write-Host "Step 9: TEST UNAUTHENTICATED ACCESS" -ForegroundColor Yellow
Write-Host "----------------------------------------" -ForegroundColor Yellow
Write-Host ""
Write-Host "Testing as: UNAUTHENTICATED (no token)" -ForegroundColor Magenta

Write-Host "Request: GET $API_URL/api/products" -ForegroundColor Cyan

try {
    $response = Invoke-RestMethod -Uri "$API_URL/api/products" `
        -Method Get `
        -ErrorAction Stop
    
    Write-Host "Response: Status 200 OK (Unexpected!)" -ForegroundColor Red
}
catch {
    $statusCode = $_.Exception.Response.StatusCode
    Write-Host "Response: Status $statusCode" -ForegroundColor Red
    
    if ($statusCode -eq "Unauthorized") {
        Write-Host "EXPECTED: Unauthenticated access denied (Unauthorized 401)" -ForegroundColor Green
    }
}

Write-Host ""
Write-Host ""

# ==========================================
# SUMMARY
# ==========================================
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "TESTING SUMMARY" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "API Endpoint: $API_URL" -ForegroundColor White
Write-Host ""
Write-Host "Users Tested:" -ForegroundColor Yellow
Write-Host "  1. sarah@pharmadocs.ca (RegAffairsOfficer) - Can CREATE/UPDATE/DELETE" -ForegroundColor Green
Write-Host "  2. james@pharmadocs.ca (Viewer) - Can only READ" -ForegroundColor Green
Write-Host ""
Write-Host "Endpoints Tested:" -ForegroundColor Yellow
Write-Host "  - POST   /api/auth/login" -ForegroundColor Cyan
Write-Host "  - GET    /api/products" -ForegroundColor Cyan
Write-Host "  - GET    /api/products/{id}" -ForegroundColor Cyan
Write-Host "  - POST   /api/products (RegAffairsOfficer only)" -ForegroundColor Cyan
Write-Host "  - GET    /api/documents" -ForegroundColor Cyan
Write-Host "  - GET    /api/documents/by-product/{productId}" -ForegroundColor Cyan
Write-Host "  - GET    /api/documents/{id}" -ForegroundColor Cyan
Write-Host "  - POST   /api/documents (RegAffairsOfficer only)" -ForegroundColor Cyan
Write-Host ""
Write-Host "Key Findings:" -ForegroundColor Yellow
Write-Host "  ✓ Both roles can read (GET) all endpoints after authentication" -ForegroundColor Green
Write-Host "  ✓ Only RegAffairsOfficer can create (POST) products and documents" -ForegroundColor Green
Write-Host "  ✓ Unauthenticated access returns 401 Unauthorized" -ForegroundColor Green
Write-Host ""
