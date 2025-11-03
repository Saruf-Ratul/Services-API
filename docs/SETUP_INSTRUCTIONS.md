# Setup Instructions for Services API

## Quick Start

### 1. Prerequisites

Install the following on your development machine:

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop) (for Redis)
- [Visual Studio 2022](https://visualstudio.microsoft.com/downloads/) or [VS Code](https://code.visualstudio.com/download)
- SQL Server (or access to existing SQL Server)

### 2. Clone and Restore

```bash
# Navigate to the project directory
cd D:\XSI\Services\Services

# Restore all NuGet packages
dotnet restore Services.sln

# Build the solution to verify everything compiles
dotnet build Services.sln
```

### 3. Configure Database Connections

Edit `src/Services.API/appsettings.json` and update the connection strings:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=172.168.90.16;Initial Catalog=Mobilize;Persist Security Info=True;User ID=Mobilizedba;Password=Mobilizedba;Connect Timeout=600;MultipleActiveResultSets=true;TrustServerCertificate=true",
    "JobsConnection": "Data Source=172.168.90.16;Initial Catalog=myServiceJobs;Persist Security Info=True;User ID=CecPro;Password=2gX6w@MRj4QE;Connect Timeout=600;MultipleActiveResultSets=true;TrustServerCertificate=true",
    "SchedulerConnection": "Data Source=172.168.90.16;Initial Catalog=msSchedulerV3;Persist Security Info=True;User ID=CecPro;Password=2gX6w@MRj4QE;Connect Timeout=600;MultipleActiveResultSets=true;TrustServerCertificate=true",
    "Redis": "localhost:6379"
  }
}
```

### 4. Start Redis (Docker)

```bash
docker run -d -p 6379:6379 --name services-redis redis:7-alpine
```

Or using docker-compose from the infrastructure folder:

```bash
cd infrastructure/docker
docker-compose up -d redis
```

### 5. Run the Application

```bash
cd src/Services.API
dotnet run
```

Or run from Visual Studio by setting `Services.API` as the startup project.

### 6. Access the Application

Once running, you can access:

- **REST API Swagger UI**: https://localhost:7148/swagger
- **Health Check**: https://localhost:7148/health
- **API Base URL**: https://localhost:7148
- **SOAP Endpoints**: Will be configured after processor migration

## Project Structure Overview

```
Services/
├── src/
│   ├── Services.API/              # Web API layer (controllers, SOAP endpoints)
│   ├── Services.Application/      # Business logic layer (use cases, DTOs)
│   ├── Services.Domain/           # Domain entities (database models)
│   └── Services.Infrastructure/   # Data access, external services
├── infrastructure/
│   ├── docker/                    # Docker configurations
│   ├── kubernetes/                # Kubernetes manifests
│   └── terraform/                 # Infrastructure as code
├── Services.sln                   # Solution file
├── README.md                      # Main documentation
├── MIGRATION_GUIDE.md             # How to migrate legacy processors
└── SETUP_INSTRUCTIONS.md          # This file
```

## Next Steps

### Option 1: Start Using the New Structure

The new Clean Architecture is ready to use. You can:

1. Create new features following Clean Architecture principles
2. Migrate existing processors gradually using the `MIGRATION_GUIDE.md`
3. Start building REST API controllers in `Services.API/Controllers/`

### Option 2: Migrate Legacy Processors

Follow the `MIGRATION_GUIDE.md` to migrate your existing processors from the `Processor/` folder to the new structure.

Key steps:
1. Create repository interfaces in `Services.Application/Interfaces/Repositories/`
2. Implement repositories in `Services.Infrastructure/Repositories/`
3. Create use case handlers in `Services.Application/UseCases/`
4. Update SOAP service to use MediatR

### Option 3: Continue Using Legacy Code

The old code in the root directory (`Processor/`, `Entity/`, `DeviceService.asmx.cs`) still exists and can be used as-is. You can migrate gradually.

## Common Issues

### Issue: Can't connect to SQL Server

**Solution**: Ensure SQL Server is accessible and connection string is correct. Check firewall rules if needed.

### Issue: Redis connection fails

**Solution**: Make sure Redis container is running:
```bash
docker ps
docker logs services-redis
```

### Issue: SSL certificate errors

**Solution**: In development, you can disable SSL validation in appsettings.Development.json. Never do this in production.

### Issue: Migration errors

**Solution**: Run migrations manually:
```bash
cd src/Services.API
dotnet ef database update --project ../Services.Infrastructure
```

## Development Tips

1. **Use Swagger**: The Swagger UI is great for testing REST endpoints during development
2. **Enable Detailed Logging**: Check `Program.cs` for logging configuration
3. **Health Checks**: Use `/health` endpoint to verify database and Redis connections
4. **Hot Reload**: Use `dotnet watch run` for automatic restarts on code changes

## Production Deployment

See the `README.md` for details on:
- Docker deployment
- Kubernetes deployment  
- Terraform infrastructure setup

## Getting Help

- Check `MIGRATION_GUIDE.md` for processor migration examples
- Review `README.md` for architecture overview
- Check linter errors: should show 0 errors

## Database Schema Preservation

⚠️ **IMPORTANT**: All database table and column names are preserved exactly as in the original database. No schema changes were made. The `ApplicationDbContext` uses exact table names with schema prefixes.

## Testing

Run tests (when added):
```bash
dotnet test
```

## Build for Production

```bash
dotnet publish Services.sln -c Release -o ./publish
```

## Contact

For issues or questions about the new architecture, refer to the documentation files or contact the development team.

