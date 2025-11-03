# Services API - Clean Architecture with .NET 9

A modernized SOAP/REST API service built with Clean Architecture principles, .NET 9, OAuth2.1/OIDC with MFA support, Redis caching, and Kubernetes/Docker deployment.

## Architecture

```
src/
├── Services.API/              # Presentation Layer (Controllers, SOAP endpoints)
├── Services.Application/      # Application Layer (Use Cases, DTOs, Interfaces)
├── Services.Domain/           # Domain Layer (Entities, Domain Logic)
└── Services.Infrastructure/   # Infrastructure Layer (Data Access, External Services)

infrastructure/
├── docker/                    # Docker configuration
├── kubernetes/                # Kubernetes manifests
└── terraform/                 # Infrastructure as Code
```

## Features

- ✅ Clean Architecture with separation of concerns
- ✅ .NET 9 (ASP.NET Core)
- ✅ SQL Server with Entity Framework Core
- ✅ Redis for caching and queues
- ✅ OAuth2.1/OIDC authentication with MFA support
- ✅ SOAP API support (SoapCore)
- ✅ REST API with Swagger/OpenAPI
- ✅ Docker containerization
- ✅ Kubernetes deployment
- ✅ Terraform infrastructure as code
- ✅ Health checks
- ✅ Database schema preserved (table/column names unchanged)

## Prerequisites

- .NET 9 SDK
- Docker Desktop
- Kubernetes cluster (for deployment)
- Terraform (for infrastructure)
- SQL Server
- Redis

## Getting Started

### Local Development

1. **Clone and restore dependencies:**
   ```bash
   dotnet restore Services.sln
   ```

2. **Update connection strings in `src/Services.API/appsettings.json`**

3. **Run Redis (Docker):**
   ```bash
   docker run -d -p 6379:6379 redis:7-alpine
   ```

4. **Run the application:**
   ```bash
   cd src/Services.API
   dotnet run
   ```

5. **Access:**
   - API: `https://localhost:7148`
   - Swagger: `https://localhost:7148/swagger`
   - SOAP: `https://localhost:7148/DeviceService.asmx`
   - Health: `https://localhost:7148/health`

### Docker Deployment

```bash
cd infrastructure/docker
docker-compose up -d
```

### Kubernetes Deployment

1. **Create namespace:**
   ```bash
   kubectl create namespace services
   ```

2. **Create secrets:**
   ```bash
   kubectl create secret generic services-secrets \
     --from-literal=db-connection-string='your-connection-string' \
     --from-literal=jwt-secret-key='your-secret-key' \
     -n services
   ```

3. **Deploy:**
   ```bash
   kubectl apply -f infrastructure/kubernetes/deployment.yaml
   ```

### Terraform Infrastructure

1. **Initialize:**
   ```bash
   cd infrastructure/terraform
   terraform init
   ```

2. **Plan:**
   ```bash
   terraform plan
   ```

3. **Apply:**
   ```bash
   terraform apply
   ```

## Configuration

### OAuth2.1/OIDC Setup

Configure in `appsettings.json`:

```json
{
  "OidcSettings": {
    "Authority": "https://your-identity-provider.com",
    "ClientId": "your-client-id",
    "ClientSecret": "your-client-secret",
    "RequireMFA": true
  }
}
```

### MFA Support

The API supports Multi-Factor Authentication through OIDC. Users with MFA enabled will have the `amr` claim set to `mfa`.

### Redis Configuration

```json
{
  "ConnectionStrings": {
    "Redis": "localhost:6379"
  }
}
```

## API Endpoints

### REST Endpoints
- `GET /api/customers` - Get customers
- `GET /api/appointments` - Get appointments
- `GET /api/invoices` - Get invoices
- `POST /api/customers` - Create customer
- `PUT /api/appointments/{id}` - Update appointment

### SOAP Endpoints
- `/DeviceService.asmx` - Full SOAP service with all original methods

## Database Schema

**⚠️ IMPORTANT: All table and column names are preserved exactly as in the original database.**

The following databases are used:
- `Mobilize` - Main database
- `myServiceJobs` - Jobs database  
- `msSchedulerV3` - Scheduler database
- `XinatorCentral` - Central database

## Development

### Adding New Features

1. **Domain Layer**: Add entities in `Services.Domain/Entities/`
2. **Application Layer**: Add DTOs and interfaces in `Services.Application/`
3. **Infrastructure Layer**: Implement repositories in `Services.Infrastructure/`
4. **API Layer**: Add controllers in `Services.API/Controllers/`

### Testing

```bash
dotnet test
```

## Deployment

### Production Checklist

- [ ] Update connection strings
- [ ] Configure OAuth2.1/OIDC settings
- [ ] Set up Redis cluster
- [ ] Configure Kubernetes secrets
- [ ] Set up SSL certificates
- [ ] Configure health checks
- [ ] Set up monitoring and logging

## Security

- OAuth2.1/OIDC with MFA support
- JWT token validation
- HTTPS enforced
- SQL injection protection (parameterized queries)
- CORS configured
- Health check endpoints

## Monitoring

- Health checks at `/health`
- Application insights (configurable)
- Logging with Serilog

## License

Proprietary - All rights reserved

## Support

For issues or questions, contact the development team.

