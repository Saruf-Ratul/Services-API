# IIS Publish Script for Services API

Write-Host "=========================================" -ForegroundColor Cyan
Write-Host "IIS Publishing Script" -ForegroundColor Yellow
Write-Host "=========================================" -ForegroundColor Cyan
Write-Host ""

# Set paths
$projectPath = "src\Services.API\Services.API.csproj"
$publishPath = "publish\IIS"

# Clean previous publish
Write-Host "Cleaning previous publish..." -ForegroundColor Yellow
if (Test-Path $publishPath) {
    Remove-Item -Path $publishPath -Recurse -Force -ErrorAction SilentlyContinue
    Start-Sleep -Seconds 2
}
New-Item -ItemType Directory -Path $publishPath -Force | Out-Null

# Publish project
Write-Host "Publishing project..." -ForegroundColor Yellow
Write-Host "Configuration: Release" -ForegroundColor Gray
Write-Host "Target: IIS (Windows Server)" -ForegroundColor Gray
Write-Host "Output: $publishPath" -ForegroundColor Gray
Write-Host ""

$publishResult = dotnet publish $projectPath -c Release -o $publishPath --self-contained false -v minimal

if ($LASTEXITCODE -ne 0) {
    Write-Host "Publish failed!" -ForegroundColor Red
    exit 1
}

# Copy web.config
Write-Host "Copying web.config..." -ForegroundColor Yellow
Copy-Item -Path "src\Services.API\web.config" -Destination "$publishPath\web.config" -Force

# Verify critical files
Write-Host "Verifying published files..." -ForegroundColor Yellow
$requiredFiles = @("Services.API.dll", "Services.API.exe", "web.config", "appsettings.json", "appsettings.Production.json")

$allPresent = $true
foreach ($file in $requiredFiles) {
    $filePath = Join-Path $publishPath $file
    if (Test-Path $filePath) {
        Write-Host "  OK: $file" -ForegroundColor Green
    } else {
        Write-Host "  MISSING: $file" -ForegroundColor Red
        $allPresent = $false
    }
}

# Summary
Write-Host ""
Write-Host "=========================================" -ForegroundColor Cyan
if ($allPresent) {
    Write-Host "PUBLISH SUCCESSFUL!" -ForegroundColor Green
    Write-Host "=========================================" -ForegroundColor Cyan
    Write-Host ""
    
    $publishSize = (Get-ChildItem -Path $publishPath -Recurse -File | Measure-Object -Property Length -Sum).Sum / 1MB
    Write-Host "Publish Summary:" -ForegroundColor Yellow
    Write-Host "  Location: $((Resolve-Path $publishPath).Path)" -ForegroundColor White
    Write-Host "  Size: $([math]::Round($publishSize, 2)) MB" -ForegroundColor White
    Write-Host ""
    Write-Host "Next Steps:" -ForegroundColor Yellow
    Write-Host "  1. Copy folder to Windows Server" -ForegroundColor White
    Write-Host "  2. Configure IIS Application Pool" -ForegroundColor White
    Write-Host "  3. Create IIS Website/Application" -ForegroundColor White
    Write-Host "  4. Set permissions" -ForegroundColor White
    Write-Host "  5. Review: publish\IIS\DEPLOYMENT_INSTRUCTIONS.md" -ForegroundColor White
    Write-Host ""
    Write-Host "Ready for IIS deployment!" -ForegroundColor Green
} else {
    Write-Host "PUBLISH INCOMPLETE!" -ForegroundColor Red
    Write-Host "=========================================" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "Some required files are missing. Please check errors above." -ForegroundColor Yellow
    exit 1
}

Write-Host ""
