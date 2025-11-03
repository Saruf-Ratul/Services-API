# Migration Guide: Legacy to Clean Architecture

## Overview

This guide explains how to migrate the existing processors and services from the legacy structure to the new Clean Architecture structure.

## Architecture Comparison

### Legacy Structure
```
Services/
├── DeviceService.asmx.cs     (SOAP service with all logic)
├── Processor/                (Business logic mixed with data access)
├── Entity/                   (Entities with System.Web dependencies)
├── Models/                   (DTOs)
└── Database.cs               (Data access)
```

### New Structure
```
src/
├── Services.API/             (Presentation - Controllers, SOAP endpoints)
├── Services.Application/     (Use Cases, DTOs, Interfaces)
├── Services.Domain/          (Pure domain entities)
└── Services.Infrastructure/  (Data access, external services)
```

## Migration Steps

### Step 1: Migrate Processors to Application Layer

1. **Create Interface in Application Layer:**

   Create `src/Services.Application/Interfaces/I{ProcessorName}Service.cs`:
   ```csharp
   namespace Services.Application.Interfaces;
   
   public interface ILoginService
   {
       Task<ResponseDto> VerifyUserAsync(LoginRequestDto request);
   }
   ```

2. **Create Use Case Handler:**

   Create `src/Services.Application/UseCases/Auth/VerifyUserCommand.cs`:
   ```csharp
   using MediatR;
   using Services.Application.DTOs;
   using Services.Application.Interfaces;
   
   namespace Services.Application.UseCases.Auth;
   
   public class VerifyUserCommand : IRequest<ResponseDto>
   {
       public string UserName { get; set; } = string.Empty;
       public string Password { get; set; } = string.Empty;
       public int AppType { get; set; }
   }
   
   public class VerifyUserCommandHandler : IRequestHandler<VerifyUserCommand, ResponseDto>
   {
       private readonly ILoginRepository _repository;
       
       public VerifyUserCommandHandler(ILoginRepository repository)
       {
           _repository = repository;
       }
       
       public async Task<ResponseDto> Handle(VerifyUserCommand request, CancellationToken cancellationToken)
       {
           // Implementation using repository
           return await _repository.VerifyUserAsync(request);
       }
   }
   ```

### Step 2: Create Repositories in Infrastructure Layer

1. **Create Repository Interface in Application:**

   `src/Services.Application/Interfaces/Repositories/ILoginRepository.cs`:
   ```csharp
   namespace Services.Application.Interfaces.Repositories;
   
   public interface ILoginRepository
   {
       Task<ResponseDto> VerifyUserAsync(LoginRequestDto request);
   }
   ```

2. **Implement Repository in Infrastructure:**

   `src/Services.Infrastructure/Repositories/LoginRepository.cs`:
   ```csharp
   using Microsoft.EntityFrameworkCore;
   using Services.Application.DTOs;
   using Services.Application.Interfaces.Repositories;
   using Services.Infrastructure.Data;
   
   namespace Services.Infrastructure.Repositories;
   
   public class LoginRepository : ILoginRepository
   {
       private readonly ApplicationDbContext _context;
       
       public LoginRepository(ApplicationDbContext context)
       {
           _context = context;
       }
       
       public async Task<ResponseDto> VerifyUserAsync(LoginRequestDto request)
       {
           // Use Dapper or EF Core to query
           // Preserve exact SQL and table/column names
           var query = @"SELECT c.CompanyIdInt, c.TimeZone, c.CompanyID, c.CompanyGUID, 
                        c.CompanyName, c.CompanyTag, u.UserID, u.Password, u.email, 
                        u.FirstName, u.LastName, u.id 
                        FROM [XinatorCentral].[dbo].[tbl_User] u 
                        INNER JOIN msSchedulerV3.dbo.tbl_Company c 
                        ON u.CompanyID = c.CompanyID 
                        WHERE u.Password = @Password AND u.UserID = @UserName";
           
           // Execute query and map to ResponseDto
           // ...
       }
   }
   ```

### Step 3: Register Services in Program.cs

```csharp
// In src/Services.API/Program.cs

// Register repositories
builder.Services.AddScoped<ILoginRepository, LoginRepository>();

// Register application services
builder.Services.AddScoped<ILoginService, LoginService>();
```

### Step 4: Update SOAP Service

1. **Create SOAP Service in API Layer:**

   `src/Services.API/Services/DeviceService.cs`:
   ```csharp
   using MediatR;
   using Services.Application.DTOs;
   using Services.Application.UseCases.Auth;
   using SoapCore.ServiceModel;
   using System.ServiceModel;
   
   namespace Services.API.Services;
   
   [ServiceContract(Namespace = "http://tempuri.org/")]
   public interface IDeviceService
   {
       [OperationContract]
       ResponseDto VerifyUser(string userName, string password, int appType);
   }
   
   [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
   public class DeviceService : IDeviceService
   {
       private readonly IMediator _mediator;
       
       public DeviceService(IMediator mediator)
       {
           _mediator = mediator;
       }
       
       public ResponseDto VerifyUser(string userName, string password, int appType)
       {
           var command = new VerifyUserCommand
           {
               UserName = userName,
               Password = password,
               AppType = appType
           };
           
           return _mediator.Send(command).Result;
       }
   }
   ```

2. **Register SOAP Endpoint:**

   In `Program.cs`:
   ```csharp
   app.UseSoapEndpoint<DeviceService>("/DeviceService.asmx", 
       new SoapEncoderOptions(), SoapSerializer.XmlSerializer);
   ```

## Key Principles

### 1. Database Schema Preservation
- **Never change table names**
- **Never change column names**
- Use exact SQL queries from legacy code
- Map to entities that match database structure

### 2. Dependency Direction
- Domain → No dependencies
- Application → Domain only
- Infrastructure → Application, Domain
- API → Application, Infrastructure

### 3. Data Access Pattern
- Use Dapper for complex SQL (preserves existing queries)
- Use EF Core for standard CRUD
- Always use parameterized queries (prevent SQL injection)

## Processor Migration Checklist

For each processor in `Processor/` folder:

- [ ] Create interface in `Services.Application/Interfaces/`
- [ ] Create DTOs in `Services.Application/DTOs/`
- [ ] Create use case handlers in `Services.Application/UseCases/`
- [ ] Create repository interface in `Services.Application/Interfaces/Repositories/`
- [ ] Implement repository in `Services.Infrastructure/Repositories/`
- [ ] Register in DI container (`Program.cs`)
- [ ] Update SOAP service to use MediatR
- [ ] Add unit tests

## Example: Migrating LoginProcessor

### Before (Legacy):
```csharp
// Processor/LoginProcessor.cs
public class LoginProcessor
{
    public Response VerifyUser(RequestEntity requestEntity)
    {
        Database db = new Database();
        // Direct database access
    }
}
```

### After (Clean Architecture):
```csharp
// Application Layer
public interface ILoginService
{
    Task<ResponseDto> VerifyUserAsync(LoginRequestDto request);
}

// Infrastructure Layer
public class LoginRepository : ILoginRepository
{
    private readonly ApplicationDbContext _context;
    // Use EF Core or Dapper
}
```

## Testing

After migration:
1. Test all SOAP endpoints
2. Test REST endpoints
3. Verify database queries match original
4. Check performance (Redis caching)
5. Verify OAuth2.1/OIDC authentication

## Notes

- All existing SOAP endpoints must remain functional
- Database schema is immutable
- Redis caching can be added to frequently accessed data
- Use async/await throughout new code
- Follow CQRS pattern with MediatR

