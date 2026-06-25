# ============================================================
#  PharmaDocs API — Automated Test Script
#  Version: 0.9.0  |  Updated: 2026-06-24
#
#  Controllers tested:
#    AuthController                POST /api/auth/login
#    ProductsController            GET | GET/{id} | POST | PUT/{id} | PATCH/{id}/archive
#    DocumentsController           GET | GET/{id} | GET/by-product/{id} | POST | PUT/{id} | PATCH/{id}/archive
#    SubmissionPackagesController  GET | GET/{id} | GET/by-product/{id} | POST | PUT/{id} | PATCH/{id}/status | PATCH/{id}/archive
#    AuditLogsController           GET /api/auditlogs?entityType=&entityId=
#
#  Run: .\test-api.ps1
# ============================================================

$BaseUrl = "http://localhost:5046"
$Pass    = 0
$Fail    = 0

function Assert($Label, $Actual, $Expected) {
    if ($Actual -eq $Expected) {
        Write-Host "  [PASS] $Label" -ForegroundColor Green
        $script:Pass++
    } else {
        Write-Host "  [FAIL] $Label -- expected $Expected, got $Actual" -ForegroundColor Red
        $script:Fail++
    }
}

function Call($Method, $Url, $Token = $null, $Body = $null) {
    $headers = @{ "Content-Type" = "application/json" }
    if ($Token) { $headers["Authorization"] = "Bearer $Token" }
    try {
        $params = @{ Method = $Method; Uri = $Url; Headers = $headers; ErrorAction = "Stop" }
        if ($Body) { $params["Body"] = ($Body | ConvertTo-Json -Depth 10) }
        return (Invoke-WebRequest @params).StatusCode
    } catch {
        return [int]$_.Exception.Response.StatusCode
    }
}

# ============================================================
Write-Host "`n=== STEP 1: Auth ===" -ForegroundColor Cyan
# ============================================================

$OfficerToken = $null
$ViewerToken  = $null

try {
    $r = Invoke-WebRequest -Method POST -Uri "$BaseUrl/api/auth/login" `
         -Headers @{"Content-Type"="application/json"} `
         -Body '{"email":"sarah@pharmadocs.ca","password":"Demo1234!"}' -ErrorAction Stop
    $OfficerToken = ($r.Content | ConvertFrom-Json).token
    Assert "Login as RegAffairsOfficer (Sarah)" $r.StatusCode 200
} catch { Assert "Login as RegAffairsOfficer (Sarah)" 0 200 }

try {
    $r = Invoke-WebRequest -Method POST -Uri "$BaseUrl/api/auth/login" `
         -Headers @{"Content-Type"="application/json"} `
         -Body '{"email":"james@pharmadocs.ca","password":"Demo1234!"}' -ErrorAction Stop
    $ViewerToken = ($r.Content | ConvertFrom-Json).token
    Assert "Login as Viewer (James)" $r.StatusCode 200
} catch { Assert "Login as Viewer (James)" 0 200 }

Assert "Login with bad credentials returns 401" `
    (Call "POST" "$BaseUrl/api/auth/login" -Body @{email="x@x.com";password="wrong"}) 401

# ============================================================
Write-Host "`n=== STEP 2: Products — read ===" -ForegroundColor Cyan
# ============================================================

Assert "GET /api/products -- officer 200" (Call "GET" "$BaseUrl/api/products" $OfficerToken) 200
Assert "GET /api/products -- viewer 200"  (Call "GET" "$BaseUrl/api/products" $ViewerToken) 200
Assert "GET /api/products -- unauth 401"  (Call "GET" "$BaseUrl/api/products") 401
Assert "GET /api/products/{id} -- officer 200" `
    (Call "GET" "$BaseUrl/api/products/aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa" $OfficerToken) 200
Assert "GET /api/products/{id} -- viewer 200" `
    (Call "GET" "$BaseUrl/api/products/aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa" $ViewerToken) 200
Assert "GET /api/products/{id} -- unknown GUID 404" `
    (Call "GET" "$BaseUrl/api/products/00000000-0000-0000-0000-000000000000" $OfficerToken) 404

# ============================================================
Write-Host "`n=== STEP 3: Products — write ===" -ForegroundColor Cyan
# ============================================================

$newProduct = @{
    name="Aspirin 500mg"; din="00000001"; npn=$null
    medicinalIngredient="Acetylsalicylic Acid"; manufacturer="Test Corp"
    dosageForm="Tablet"; routeOfAdministration="Oral"; therapeuticCategory="Analgesic"
}
Assert "POST /api/products -- officer 201" (Call "POST" "$BaseUrl/api/products" $OfficerToken $newProduct) 201
Assert "POST /api/products -- viewer 403"  (Call "POST" "$BaseUrl/api/products" $ViewerToken $newProduct) 403

$updProduct = @{
    name="Atorvastatin 40mg"; din="02241127"; npn=$null
    medicinalIngredient="Atorvastatin Calcium"; manufacturer="Apotex Inc."
    dosageForm="Tablet"; routeOfAdministration="Oral"; therapeuticCategory="Antihyperlipidemic"
}
Assert "PUT /api/products/{id} -- officer 200" `
    (Call "PUT" "$BaseUrl/api/products/bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb" $OfficerToken $updProduct) 200
Assert "PUT /api/products/{id} -- viewer 403" `
    (Call "PUT" "$BaseUrl/api/products/bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb" $ViewerToken $updProduct) 403

Assert "PATCH /api/products/{id}/archive -- viewer 403" `
    (Call "PATCH" "$BaseUrl/api/products/aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa/archive" $ViewerToken) 403
Assert "PATCH /api/products/{id}/archive -- officer 204" `
    (Call "PATCH" "$BaseUrl/api/products/aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa/archive" $OfficerToken) 204
Assert "PATCH /api/products/{id}/archive -- already archived 404" `
    (Call "PATCH" "$BaseUrl/api/products/aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa/archive" $OfficerToken) 404

# ============================================================
Write-Host "`n=== STEP 4: Documents — read ===" -ForegroundColor Cyan
# ============================================================

Assert "GET /api/documents -- officer 200" (Call "GET" "$BaseUrl/api/documents" $OfficerToken) 200
Assert "GET /api/documents -- viewer 200"  (Call "GET" "$BaseUrl/api/documents" $ViewerToken) 200
Assert "GET /api/documents -- unauth 401"  (Call "GET" "$BaseUrl/api/documents") 401
Assert "GET /api/documents/by-product/{id} -- officer 200" `
    (Call "GET" "$BaseUrl/api/documents/by-product/aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa" $OfficerToken) 200
Assert "GET /api/documents/by-product/{id} -- viewer 200" `
    (Call "GET" "$BaseUrl/api/documents/by-product/aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa" $ViewerToken) 200
Assert "GET /api/documents/{id} -- officer 200" `
    (Call "GET" "$BaseUrl/api/documents/cccccccc-cccc-cccc-cccc-cccccccccccc" $OfficerToken) 200

# ============================================================
Write-Host "`n=== STEP 5: Documents — write ===" -ForegroundColor Cyan
# ============================================================

$newDoc = @{
    title="Test CoA v1.0"; type="CertificateOfAnalysis"; status="Draft"
    version="1.0"; date="2026-06-24"; notes="Test via PS1"
    productId="aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"
}
Assert "POST /api/documents -- officer 201" (Call "POST" "$BaseUrl/api/documents" $OfficerToken $newDoc) 201
Assert "POST /api/documents -- viewer 403"  (Call "POST" "$BaseUrl/api/documents" $ViewerToken $newDoc) 403

$updDoc = @{
    title="CoA v1.1"; type="CertificateOfAnalysis"; status="Approved"
    version="1.1"; date="2026-06-24"; notes="Updated"
}
Assert "PUT /api/documents/{id} -- officer 200" `
    (Call "PUT" "$BaseUrl/api/documents/cccccccc-cccc-cccc-cccc-cccccccccccc" $OfficerToken $updDoc) 200
Assert "PUT /api/documents/{id} -- viewer 403" `
    (Call "PUT" "$BaseUrl/api/documents/cccccccc-cccc-cccc-cccc-cccccccccccc" $ViewerToken $updDoc) 403

Assert "PATCH /api/documents/{id}/archive -- viewer 403" `
    (Call "PATCH" "$BaseUrl/api/documents/cccccccc-cccc-cccc-cccc-cccccccccccc/archive" $ViewerToken) 403
Assert "PATCH /api/documents/{id}/archive -- officer 204" `
    (Call "PATCH" "$BaseUrl/api/documents/cccccccc-cccc-cccc-cccc-cccccccccccc/archive" $OfficerToken) 204

# ============================================================
Write-Host "`n=== STEP 6: Submission Packages — read ===" -ForegroundColor Cyan
# ============================================================

Assert "GET /api/submissionpackages -- officer 200" `
    (Call "GET" "$BaseUrl/api/submissionpackages" $OfficerToken) 200
Assert "GET /api/submissionpackages -- viewer 200" `
    (Call "GET" "$BaseUrl/api/submissionpackages" $ViewerToken) 200
Assert "GET /api/submissionpackages -- unauth 401" `
    (Call "GET" "$BaseUrl/api/submissionpackages") 401
Assert "GET /api/submissionpackages/{id} -- officer 200" `
    (Call "GET" "$BaseUrl/api/submissionpackages/eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee" $OfficerToken) 200
Assert "GET /api/submissionpackages/{id} -- viewer 200" `
    (Call "GET" "$BaseUrl/api/submissionpackages/eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee" $ViewerToken) 200
Assert "GET /api/submissionpackages/by-product/{id} -- officer 200" `
    (Call "GET" "$BaseUrl/api/submissionpackages/by-product/aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa" $OfficerToken) 200

# ============================================================
Write-Host "`n=== STEP 7: Submission Packages — write ===" -ForegroundColor Cyan
# ============================================================

$newPkg = @{
    submissionType="NDS"; regulatoryBody="Health Canada"
    productId="bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"; targetDate="2026-12-31"
}
Assert "POST /api/submissionpackages -- officer 201" `
    (Call "POST" "$BaseUrl/api/submissionpackages" $OfficerToken $newPkg) 201
Assert "POST /api/submissionpackages -- viewer 403" `
    (Call "POST" "$BaseUrl/api/submissionpackages" $ViewerToken $newPkg) 403

$updPkg = @{ regulatoryBody="Health Canada"; targetDate="2027-03-01"; submissionDate=$null }
Assert "PUT /api/submissionpackages/{id} -- officer 200" `
    (Call "PUT" "$BaseUrl/api/submissionpackages/eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee" $OfficerToken $updPkg) 200
Assert "PUT /api/submissionpackages/{id} -- viewer 403" `
    (Call "PUT" "$BaseUrl/api/submissionpackages/eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee" $ViewerToken $updPkg) 403

$statusUpd = @{ newStatus="InProgress"; notes="Work begun" }
Assert "PATCH /api/submissionpackages/{id}/status -- officer 200" `
    (Call "PATCH" "$BaseUrl/api/submissionpackages/eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee/status" $OfficerToken $statusUpd) 200
Assert "PATCH /api/submissionpackages/{id}/status -- viewer 403" `
    (Call "PATCH" "$BaseUrl/api/submissionpackages/eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee/status" $ViewerToken $statusUpd) 403

# Archive: seeded package is now InProgress (from status patch above) so 204 expected
Assert "PATCH /api/submissionpackages/{id}/archive -- viewer 403" `
    (Call "PATCH" "$BaseUrl/api/submissionpackages/eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee/archive" $ViewerToken) 403
Assert "PATCH /api/submissionpackages/{id}/archive -- officer (InProgress) 204" `
    (Call "PATCH" "$BaseUrl/api/submissionpackages/eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee/archive" $OfficerToken) 204

# ============================================================
Write-Host "`n=== STEP 8: Audit Logs ===" -ForegroundColor Cyan
# ============================================================

# Valid entityType — both roles
Assert "GET /api/auditlogs?entityType=Product -- officer 200" `
    (Call "GET" "$BaseUrl/api/auditlogs?entityType=Product" $OfficerToken) 200
Assert "GET /api/auditlogs?entityType=Product -- viewer 200" `
    (Call "GET" "$BaseUrl/api/auditlogs?entityType=Product" $ViewerToken) 200
Assert "GET /api/auditlogs?entityType=Product -- unauth 401" `
    (Call "GET" "$BaseUrl/api/auditlogs?entityType=Product") 401

Assert "GET /api/auditlogs?entityType=DocumentRecord -- officer 200" `
    (Call "GET" "$BaseUrl/api/auditlogs?entityType=DocumentRecord" $OfficerToken) 200
Assert "GET /api/auditlogs?entityType=SubmissionPackage -- officer 200" `
    (Call "GET" "$BaseUrl/api/auditlogs?entityType=SubmissionPackage" $OfficerToken) 200

# Valid entityType + entityId
Assert "GET /api/auditlogs?entityType=Product&entityId={id} -- 200" `
    (Call "GET" "$BaseUrl/api/auditlogs?entityType=Product&entityId=aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa" $OfficerToken) 200
Assert "GET /api/auditlogs?entityType=DocumentRecord&entityId={id} -- 200" `
    (Call "GET" "$BaseUrl/api/auditlogs?entityType=DocumentRecord&entityId=cccccccc-cccc-cccc-cccc-cccccccccccc" $OfficerToken) 200
Assert "GET /api/auditlogs?entityType=SubmissionPackage&entityId={id} -- 200" `
    (Call "GET" "$BaseUrl/api/auditlogs?entityType=SubmissionPackage&entityId=eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee" $OfficerToken) 200

# Unknown entityId — valid request, empty result
Assert "GET /api/auditlogs?entityType=Product&entityId=unknown -- 200 empty" `
    (Call "GET" "$BaseUrl/api/auditlogs?entityType=Product&entityId=99999999-9999-9999-9999-999999999999" $OfficerToken) 200

# Invalid entityType — expect 400
Assert "GET /api/auditlogs?entityType=Invalid -- 400" `
    (Call "GET" "$BaseUrl/api/auditlogs?entityType=Invalid" $OfficerToken) 400

# Missing entityType — expect 400
Assert "GET /api/auditlogs (no entityType) -- 400" `
    (Call "GET" "$BaseUrl/api/auditlogs" $OfficerToken) 400

# ============================================================
Write-Host "`n============================================================" -ForegroundColor Cyan
Write-Host "  Results: $Pass passed, $Fail failed" `
    -ForegroundColor $(if ($Fail -eq 0) { "Green" } else { "Yellow" })
Write-Host "============================================================`n" -ForegroundColor Cyan
