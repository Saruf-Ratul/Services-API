# Project Structure

## 📁 Clean Architecture Structure

```
Services/
│
├── 📄 README.md                      # Main project readme
├── 📄 Services.sln                   # Solution file
├── 📄 .gitignore                     # Git ignore rules
│
├── 📁 src/                           # 🎯 Modern .NET 9 Clean Architecture
│   ├── 📁 Services.API/             # 📡 Presentation Layer
│   │   ├── 📁 Controllers/          # 13 REST Controllers
│   │   ├── 📁 Filters/              # Exception & validation filters
│   │   ├── 📁 Middleware/           # Global exception handling
│   │   ├── 📄 Program.cs            # Startup configuration
│   │   ├── 📄 Services.API.csproj   # Project file
│   │   ├── 📄 web.config            # IIS deployment config
│   │   └── 📄 appsettings*.json     # Configuration files
│   │
│   ├── 📁 Services.Application/     # 🎨 Application Layer
│   │   ├── 📁 DTOs/                 # All DTOs
│   │   ├── 📁 Common/Mappings/      # AutoMapper profiles
│   │   └── 📄 Services.Application.csproj
│   │
│   ├── 📁 Services.Domain/          # 🏛️ Domain Layer
│   │   ├── 📁 Entities/             # All domain entities
│   │   └── 📄 Services.Domain.csproj
│   │
│   └── 📁 Services.Infrastructure/  # 🔧 Infrastructure Layer
│       ├── 📁 Data/                 # EF Core DbContext
│       ├── 📁 Services/             # Redis, external services
│       └── 📄 Services.Infrastructure.csproj
│
├── 📁 legacy/                        # 📦 Archived Legacy Code
│   ├── 📁 OldWebForms/              # Legacy ASP.NET Web Forms
│   │   ├── 📁 Controllers/
│   │   ├── 📁 Views/
│   │   ├── 📁 Scripts/
│   │   ├── 📁 Content/
│   │   ├── 📁 Areas/
│   │   ├── 📁 Dll/
│   │   ├── 📄 Global.asax
│   │   ├── 📄 Web.config
│   │   └── 📄 Services.csproj
│   │
│   ├── 📁 SoapService/              # Original SOAP Service
│   │   ├── 📄 DeviceService.asmx
│   │   ├── 📄 DeviceService.asmx.cs
│   │   └── 📄 Database.cs
│   │
│   ├── 📁 Processors/               # Original Business Logic
│   │   ├── 📄 AppointmentProcessor.cs
│   │   ├── 📄 CustomerProcessor.cs
│   │   ├── 📄 InvoiceProcessor.cs
│   │   └── ... (11 processors)
│   │
│   ├── 📁 Entities/                 # Original Entities
│   └── 📁 Models/                   # Original Models
│
├── 📁 infrastructure/                # 🚀 Infrastructure as Code
│   ├── 📁 docker/
│   │   ├── 📄 Dockerfile
│   │   └── 📄 docker-compose.yml
│   │
│   ├── 📁 kubernetes/
│   │   └── 📄 deployment.yaml
│   │
│   └── 📁 terraform/
│       ├── 📄 main.tf
│       ├── 📄 variables.tf
│       └── 📄 outputs.tf
│
├── 📁 docs/                         # 📚 Comprehensive Documentation
│   ├── 📄 README.md                 # Docs readme
│   ├── 📄 ARCHITECTURE.md           # Clean Architecture details
│   ├── 📄 SETUP_INSTRUCTIONS.md     # Development setup
│   ├── 📄 DEPLOYMENT_GUIDE.md       # Production deployment
│   ├── 📄 MIGRATION_GUIDE.md        # Processor migration
│   ├── 📄 API_ENDPOINTS_REFERENCE.md # Complete API reference
│   ├── 📄 SOAP_TO_REST_MIGRATION.md  # Migration summary
│   ├── 📄 FINAL_SETUP.md            # Quick start
│   ├── 📄 FINAL_STATUS.md           # Project status
│   └── 📄 IMPLEMENTATION_COMPLETE.md # Implementation summary
│
└── 📁 data/                         # 💾 Runtime Data
    ├── 📁 CompanyLogo/              # Company logos
    └── 📁 EmailHistoryContent/      # Email PDFs
```

---

## 🎯 Key Directories

### ✅ `src/` - Modern Development
**Purpose**: All active .NET 9 development  
**Contains**:
- Clean Architecture layers
- REST API controllers
- Modern configuration
- Latest packages

### 📦 `legacy/` - Historical Reference
**Purpose**: Preserved legacy code for migration reference  
**Contains**:
- Old .NET Framework code
- SOAP service implementation
- Original processors
- Historical entities/models

### 🚀 `infrastructure/` - Deployment
**Purpose**: Infrastructure as Code  
**Contains**:
- Docker configurations
- Kubernetes manifests
- Terraform scripts

### 📚 `docs/` - Documentation
**Purpose**: All project documentation  
**Contains**:
- 10 comprehensive guides
- API references
- Migration instructions
- Architecture diagrams

### 💾 `data/` - Runtime Data
**Purpose**: Application runtime data  
**Contains**:
- Uploaded files
- Generated PDFs
- Company assets

---

## 🏗️ Architecture Overview

```
┌─────────────────────────────────────────────────────────┐
│                    Presentation                         │
│  Services.API (Controllers, Middleware, Filters)       │
└──────────────────────┬──────────────────────────────────┘
                       ↓
┌─────────────────────────────────────────────────────────┐
│                   Application                           │
│  Services.Application (DTOs, Mappings, Use Cases)      │
└──────────────────────┬──────────────────────────────────┘
                       ↓
┌─────────────────────────────────────────────────────────┐
│                      Domain                             │
│  Services.Domain (Entities, Business Logic)            │
└──────────────────────┬──────────────────────────────────┘
                       ↓
┌─────────────────────────────────────────────────────────┐
│                  Infrastructure                         │
│  Services.Infrastructure (DB, Cache, External Services)│
└─────────────────────────────────────────────────────────┘
```

---

## 📊 File Count

| Directory | Files | Purpose |
|-----------|-------|---------|
| `src/Services.API` | ~20 | REST Controllers & API setup |
| `src/Services.Application` | ~10 | DTOs & Mappings |
| `src/Services.Domain` | ~15 | Domain Entities |
| `src/Services.Infrastructure` | ~5 | DB & Services |
| `legacy/` | ~100 | Archived legacy code |
| `infrastructure/` | ~8 | Docker, K8s, Terraform |
| `docs/` | 10 | Documentation |
| `data/` | Variable | Runtime data |

**Total Active Code**: ~50 files in `src/`  
**Legacy Reference**: ~100 files in `legacy/`

---

## 🔍 Quick Navigation

| Need to... | Look in... |
|------------|------------|
| Add new API endpoint | `src/Services.API/Controllers/` |
| Create new DTO | `src/Services.Application/DTOs/` |
| Add domain entity | `src/Services.Domain/Entities/` |
| Migrate processor | `legacy/Processors/` |
| Deploy Docker | `infrastructure/docker/` |
| Deploy Kubernetes | `infrastructure/kubernetes/` |
| Deploy Azure | `infrastructure/terraform/` |
| Read docs | `docs/` |
| Reference old SOAP | `legacy/SoapService/` |

---

## 🚫 What's Ignored

The following are excluded from git:

```
# Build artifacts
bin/
obj/

# IDE
.vs/
.idea/
*.user
*.suo

# NuGet
packages/
*.nupkg

# Runtime data (partial)
data/CompanyLogo/*
data/EmailHistoryContent/*

# Logs
logs/
*.log

# Environment
.env
appsettings.Development.json
appsettings.Production.json
```

---

## ✅ Benefits of Clean Structure

### 1. Clear Separation
- ✅ Modern vs Legacy clearly divided
- ✅ Architecture layers separated
- ✅ Documentation organized

### 2. Easy Navigation
- ✅ Root directory clean
- ✅ Logical folder grouping
- ✅ Intuitive naming

### 3. Scalable
- ✅ Easy to add new features
- ✅ Ready for team growth
- ✅ CI/CD friendly

### 4. Maintainable
- ✅ Legacy code archived
- ✅ New code isolated
- ✅ Clear migration path

---

## 🎯 Development Workflow

### 1. Start Development
```bash
cd src/Services.API
dotnet run
```

### 2. Add Feature
- API → `src/Services.API/Controllers/`
- DTOs → `src/Services.Application/DTOs/`
- Entities → `src/Services.Domain/Entities/`

### 3. Reference Legacy
- Check `legacy/Processors/` for original logic
- Review `legacy/SoapService/` for API mapping

### 4. Build & Test
```bash
dotnet build Services.sln
dotnet test
```

### 5. Deploy
- Docker → `infrastructure/docker/`
- Kubernetes → `infrastructure/kubernetes/`
- Azure → `infrastructure/terraform/`

---

## 📈 Structure Evolution

### Before Cleanup
```
❌ 200+ files scattered in root
❌ Legacy mixed with modern
❌ No clear organization
❌ Confusing for developers
```

### After Cleanup
```
✅ Clean root directory
✅ Clear separation of concerns
✅ Logical organization
✅ Developer-friendly structure
```

---

**Last Updated**: 2025-01-27  
**Status**: ✅ Clean & Production Ready

