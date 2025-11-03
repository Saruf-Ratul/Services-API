# Services API - Clean Architecture Documentation

## Overview

This project implements **Clean Architecture** principles with a four-layer structure separating concerns and maintaining independence from external frameworks.

## Architecture Layers

### 1. Domain Layer (`Services.Domain`)

**Purpose**: Pure business logic and entities, no external dependencies.

**Contains**:
- Domain entities (Appointment, Customer, Invoice, etc.)
- Pure business rules
- No database dependencies
- No framework dependencies

**Key Entities**:
```
Entities/
├── Appointment.cs
├── Customer.cs
├── Invoice.cs
├── Note.cs
├── FormTemplate.cs
├── User.cs
├── Company.cs
└── ...
```

**Rules**:
- No dependencies on other layers
- Can be used in any .NET project
- Represents the core business domain

### 2. Application Layer (`Services.Application`)

**Purpose**: Application use cases and business workflow.

**Contains**:
- DTOs (Data Transfer Objects)
- Interfaces for repositories and services
- Use case handlers (via MediatR)
- Mapping profiles (AutoMapper)
- Business validation rules

**Key Folders**:
```
Application/
├── DTOs/
│   ├── CustomerDto.cs
│   ├── AppointmentDto.cs
│   └── InvoiceDto.cs
├── Interfaces/
│   ├── Repositories/
│   └── Services/
├── UseCases/
└── Common/
    └── Mappings/
```

**Rules**:
- Depends only on Domain layer
- Defines contracts for Infrastructure
- No knowledge of external frameworks

### 3. Infrastructure Layer (`Services.Infrastructure`)

**Purpose**: External concerns and cross-cutting functionality.

**Contains**:
- Data access (EF Core, Dapper)
- Redis caching
- External API clients (Twilio, etc.)
- Email services
- File storage
- Repositories implementing interfaces from Application

**Key Folders**:
```
Infrastructure/
├── Data/
│   └── ApplicationDbContext.cs
├── Repositories/
├── Services/
│   └── RedisCacheService.cs
└── Configurations/
```

**Rules**:
- Implements interfaces from Application
- Contains all external framework code
- Can be swapped without affecting other layers

### 4. Presentation Layer (`Services.API`)

**Purpose**: HTTP endpoints and request/response handling.

**Contains**:
- Controllers (REST API)
- SOAP endpoints
- Middleware configuration
- Authentication/Authorization setup
- Swagger configuration

**Key Files**:
```
API/
├── Program.cs              # Application setup
├── Controllers/
│   └── (REST controllers)
├── Services/
│   └── DeviceService.cs    # SOAP service
└── appsettings.json
```

**Rules**:
- Coordinates with Application layer
- Handles HTTP concerns
- No business logic

## Dependency Flow

```
┌─────────────────────────────────┐
│      Services.API               │  ← Requests come here
│   (Presentation Layer)          │
└────────────┬────────────────────┘
             │ Depends on
             ↓
┌─────────────────────────────────┐
│   Services.Application          │
│   (Application Layer)           │
└────────────┬────────────────────┘
             │ Depends on
             ↓
┌─────────────────────────────────┐
│   Services.Domain               │
│   (Domain Layer)                │
│   ← Pure Business Logic         │
└─────────────────────────────────┘
             ↑ Implements interfaces
             │
┌─────────────────────────────────┐
│   Services.Infrastructure       │
│   (Infrastructure Layer)        │
└─────────────────────────────────┘
```

## Design Patterns Used

### 1. Repository Pattern

**Definition**: Abstraction over data access.

**Implementation**:
- Application layer defines `IRepository` interfaces
- Infrastructure layer implements repositories
- Keeps business logic independent of data access

**Example**:
```csharp
// Application Layer
public interface ICustomerRepository
{
    Task<Customer> GetByIdAsync(int id);
}

// Infrastructure Layer
public class CustomerRepository : ICustomerRepository
{
    private readonly ApplicationDbContext _context;
    // Implementation
}
```

### 2. CQRS (Command Query Responsibility Segregation)

**Definition**: Separates read and write operations.

**Implementation**:
- MediatR for command/query handling
- Commands for mutations (Create, Update, Delete)
- Queries for reads (Get, List, Search)

**Example**:
```csharp
// Command
public class CreateCustomerCommand : IRequest<int>
{
    public string Name { get; set; }
}

// Handler
public class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, int>
{
    // Handle the command
}
```

### 3. Dependency Injection

**Definition**: Inversion of Control for dependencies.

**Implementation**:
- Constructor injection throughout
- Services registered in `Program.cs`
- Testable and maintainable

### 4. DTO Pattern

**Definition**: Data Transfer Objects for API contracts.

**Implementation**:
- DTOs in Application layer
- AutoMapper for entity ↔ DTO conversion
- Separate domain models from API contracts

## Database Architecture

### Multi-Database Support

The system connects to **4 separate databases**:

1. **Mobilize** - Main application database
2. **myServiceJobs** - Job scheduling database
3. **msSchedulerV3** - Appointment scheduler database
4. **XinatorCentral** - Central authentication database

### Schema Preservation

**Critical Rule**: All database schema remains unchanged.

- Table names preserved exactly
- Column names preserved exactly
- Existing SQL queries compatible
- Entity Framework configured with schema prefixes

**Example Configuration**:
```csharp
modelBuilder.Entity<Appointment>().ToTable("tbl_Appointment", "msSchedulerV3");
modelBuilder.Entity<User>().ToTable("tbl_User", "XinatorCentral");
```

## Security Architecture

### OAuth2.1/OIDC Authentication

**Implementation**:
- JWT Bearer tokens
- OpenID Connect integration
- Multi-Factor Authentication (MFA) support
- Claims-based authorization

**Flow**:
1. Client requests authentication
2. Redirected to identity provider
3. User authenticates (with MFA if enabled)
4. Token returned to API
5. Subsequent requests use Bearer token

### MFA Support

**Detection**: `amr` claim contains `mfa` value
**Enforcement**: `RequireMFA` policy in authorization

## Caching Strategy

### Redis Integration

**Use Cases**:
1. **Cache**: Frequently accessed data
2. **Queue**: Background job processing
3. **Session**: User session data
4. **Rate Limiting**: API throttling

**Implementation**:
```csharp
public interface IRedisCacheService
{
    Task<T?> GetAsync<T>(string key);
    Task SetAsync<T>(string key, T value, TimeSpan? expiration);
}
```

## API Architecture

### Dual API Support

#### 1. REST API

**Characteristics**:
- RESTful endpoints
- JSON responses
- Swagger/OpenAPI documentation
- Standard HTTP methods

**Example**:
```
GET    /api/customers
POST   /api/customers
GET    /api/customers/{id}
PUT    /api/customers/{id}
DELETE /api/customers/{id}
```

#### 2. SOAP API

**Characteristics**:
- WSDL-compatible
- XML messages
- Legacy client support
- All original methods preserved

**Endpoint**: `/DeviceService.asmx`

## Deployment Architecture

### Docker

**Benefits**:
- Consistent environments
- Easy scaling
- Container orchestration

### Kubernetes

**Components**:
- API deployments (3 replicas)
- Redis deployment
- SQL Server deployment
- Service definitions
- Ingress configuration

### Terraform

**Infrastructure as Code**:
- Azure Kubernetes Service (AKS)
- Azure Redis Cache
- Azure SQL Database
- Resource groups
- Network configuration

## Testing Strategy

### Layer Testing

1. **Domain**: Unit tests for business logic
2. **Application**: Integration tests for use cases
3. **Infrastructure**: Repository tests with test database
4. **API**: End-to-end tests with TestServer

### Test Coverage Goals

- Domain: 90%+
- Application: 80%+
- Critical paths: 100%

## Performance Considerations

### Database Optimization

1. **Connection Pooling**: EF Core managed
2. **Query Optimization**: Include/Projections
3. **Indexes**: Database-level optimization
4. **Parameterized Queries**: Prevent SQL injection

### Caching Strategy

1. **Application Cache**: Redis for hot data
2. **Cache Invalidation**: TTL-based expiration
3. **Cache Warming**: Pre-load critical data

### API Performance

1. **Async/Await**: Non-blocking I/O
2. **Response Caching**: HTTP cache headers
3. **Compression**: gzip responses
4. **Rate Limiting**: Protect from abuse

## Monitoring and Logging

### Logging

- **Serilog**: Structured logging
- **Console Sink**: Development
- **File Sink**: Production logs
- **Application Insights**: Cloud logging

### Health Checks

- **Database**: Connection health
- **Redis**: Cache health
- **External APIs**: Availability checks

**Endpoint**: `/health`

## Migration Strategy

### Legacy Code Migration

**Approach**: Gradual migration, not big bang

**Steps**:
1. Keep legacy code running
2. Migrate processor by processor
3. Feature flag new implementations
4. Switch traffic gradually
5. Remove legacy code when stable

**See**: `MIGRATION_GUIDE.md` for details

## Best Practices

### Code Organization

1. One file per class
2. Clear folder structure
3. Meaningful names
4. SOLID principles

### Error Handling

1. Try-catch at boundary layers
2. Custom exception types
3. Logging all errors
4. User-friendly messages

### Configuration

1. appsettings.json for settings
2. Environment variables for secrets
3. No hardcoded values
4. Secure secret management

## Future Enhancements

### Planned Additions

1. **GraphQL API**: Alternative query interface
2. **SignalR**: Real-time updates
3. **gRPC**: High-performance inter-service
4. **Event Sourcing**: Audit trail
5. **CQRS Enhancement**: Separate read/write databases

## Compliance

### Security Standards

- OWASP Top 10 compliance
- GDPR considerations
- Data encryption at rest
- Data encryption in transit
- Regular security audits

### Performance Standards

- Sub-200ms average response time
- 99.9% uptime target
- Horizontal scaling ready
- Auto-scaling configured

## Documentation

- **README.md**: Quick start
- **ARCHITECTURE.md**: This file
- **MIGRATION_GUIDE.md**: Migration process
- **SETUP_INSTRUCTIONS.md**: Detailed setup
- **Inline Comments**: Code documentation

## Team Guidelines

### Pull Request Requirements

1. All tests passing
2. Code reviewed
3. Documentation updated
4. No linter errors
5. Architecture compliant

### Commit Messages

Format: `[Layer] Description`

Examples:
- `[Domain] Add new appointment entity`
- `[Application] Implement customer creation use case`
- `[Infrastructure] Add Redis cache service`
- `[API] Add authentication endpoint`

## Questions?

Refer to:
- Architecture diagrams in this file
- Code examples in migration guide
- Team lead for architecture decisions
- Tech radar for approved technologies

