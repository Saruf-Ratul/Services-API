# Services API Startup Script

Write-Host "🚀 Starting Services API..." -ForegroundColor Cyan
Write-Host ""

# Navigate to project directory
Set-Location $PSScriptRoot

# Check if dotnet is available
if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
    Write-Host "❌ .NET SDK not found. Please install .NET 9 SDK." -ForegroundColor Red
    exit 1
}

# Display .NET version
$dotnetVersion = dotnet --version
Write-Host "✅ .NET Version: $dotnetVersion" -ForegroundColor Green

# Check if project exists
if (-not (Test-Path "src/Services.API/Services.API.csproj")) {
    Write-Host "❌ Project not found!" -ForegroundColor Red
    exit 1
}

Write-Host "📦 Building project..." -ForegroundColor Yellow
dotnet build src/Services.API/Services.API.csproj -c Release --no-restore

if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Build failed!" -ForegroundColor Red
    exit 1
}

Write-Host "✅ Build successful!" -ForegroundColor Green
Write-Host ""
Write-Host "🌐 Starting Services API..." -ForegroundColor Cyan
Write-Host "📝 Swagger UI will be available at:" -ForegroundColor Yellow
Write-Host "   HTTPS: https://localhost:7148/swagger" -ForegroundColor White
Write-Host "   HTTP:  http://localhost:5148/swagger" -ForegroundColor White
Write-Host ""
Write-Host "🛑 Press Ctrl+C to stop the server" -ForegroundColor Yellow
Write-Host ""

# Run the application
dotnet run --project src/Services.API

