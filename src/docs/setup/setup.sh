
#!/bin/bash

echo "🚀 University Management System - Setup Script"
echo "================================================"

# Check if .NET 9.0 SDK is installed
if ! command -v dotnet &> /dev/null; then
    echo "❌ .NET SDK not found. Please install .NET 9.0 SDK first."
    exit 1
fi

echo "✅ .NET SDK found: $(dotnet --version)"

# Restore NuGet packages
echo ""
echo "📦 Restoring NuGet packages..."
dotnet restore

# Build solution
echo ""
echo "🔨 Building solution..."
dotnet build --configuration Release

# Run database migrations
echo ""
echo "📊 Running database migrations..."
dotnet ef database update --project src/Infrastructure/UniversityMS.Infrastructure --startup-project src/Presentation/UniversityMS.Api

echo ""
echo "✅ Setup completed successfully!"
echo ""
echo "To run the application:"
echo "  dotnet run --project src/Presentation/UniversityMS.Api"
echo ""
echo "Or with Docker:"
echo "  docker-compose up -d"
echo ""
echo "API will be available at: https://localhost:5001"
echo "Swagger UI: https://localhost:5001/swagger"
