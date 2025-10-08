
Write-Host "🚀 University Management System - Setup Script" -ForegroundColor Cyan
Write-Host "================================================" -ForegroundColor Cyan
Write-Host ""

# Check if .NET 9.0 SDK is installed
try {
    $dotnetVersion = dotnet --version
    Write-Host "✅ .NET SDK found: $dotnetVersion" -ForegroundColor Green
}
catch {
    Write-Host "❌ .NET SDK not found. Please install .NET 9.0 SDK first." -ForegroundColor Red
    exit 1
}

# Restore NuGet packages
Write-Host ""
Write-Host "📦 Restoring NuGet packages..." -ForegroundColor Yellow
dotnet restore

# Build solution
Write-Host ""
Write-Host "🔨 Building solution..." -ForegroundColor Yellow
dotnet build --configuration Release

# Run database migrations
Write-Host ""
Write-Host "📊 Running database migrations..." -ForegroundColor Yellow
dotnet ef database update --project src/Infrastructure/UniversityMS.Infrastructure --startup-project src/Presentation/UniversityMS.Api

Write-Host ""
Write-Host "✅ Setup completed successfully!" -ForegroundColor Green
Write-Host ""
Write-Host "To run the application:" -ForegroundColor Cyan
Write-Host "  dotnet run --project src/Presentation/UniversityMS.Api" -ForegroundColor White
Write-Host ""
Write-Host "Or with Docker:" -ForegroundColor Cyan
Write-Host "  docker-compose up -d" -ForegroundColor White
Write-Host ""
Write-Host "API will be available at: https://localhost:5001" -ForegroundColor Yellow
Write-Host "Swagger UI: https://localhost:5001/swagger" -ForegroundColor Yellow