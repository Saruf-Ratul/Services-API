# Clean Structure Reorganization Plan

## Current Issues
1. Legacy .NET Framework files mixed with .NET 9 Clean Architecture
2. Old build artifacts in root (bin/, obj/, etc.)
3. Documentation scattered in root
4. Old Web Forms/Controllers mixed with API
5. Legacy processor files should be archived

## Proposed Structure

```
Services/
├── src/                               # Modern .NET 9 Clean Architecture
│   ├── Services.API/
│   ├── Services.Application/
│   ├── Services.Domain/
│   └── Services.Infrastructure/
│
├── legacy/                            # Archived .NET Framework code
│   ├── OldWebForms/
│   │   ├── Controllers/
│   │   ├── Views/
│   │   ├── Scripts/
│   │   ├── Content/
│   │   ├── Global.asax
│   │   ├── Web.config
│   │   └── ...
│   ├── SoapService/
│   │   ├── DeviceService.asmx.cs
│   │   ├── DeviceService.asmx
│   │   └── Database.cs
│   ├── Processors/
│   │   └── [All processor files]
│   ├── Entities/
│   │   └── [All entity files]
│   ├── Models/
│   │   └── [All model files]
│   ├── Areas/
│   ├── App_Start/
│   ├── Dll/
│   └── Services.csproj
│
├── infrastructure/                    # IaC - Docker, K8s, Terraform
│   ├── docker/
│   ├── kubernetes/
│   └── terraform/
│
├── docs/                              # All documentation
│   ├── README.md
│   ├── ARCHITECTURE.md
│   ├── SETUP_INSTRUCTIONS.md
│   ├── DEPLOYMENT_GUIDE.md
│   ├── MIGRATION_GUIDE.md
│   ├── API_ENDPOINTS_REFERENCE.md
│   └── ...
│
├── data/                              # Runtime data (gitignored)
│   ├── logs/
│   ├── EmailHistoryContent/
│   └── CompanyLogo/
│
├── Services.sln                       # Solution file
├── .gitignore                         # Updated
└── CLEAN_STRUCTURE_PLAN.md            # This file
```

## Cleanup Steps

### Phase 1: Create Directories
- [x] Create `legacy/` folder
- [x] Create `legacy/OldWebForms/` folder
- [x] Create `legacy/SoapService/` folder
- [x] Create `legacy/Processors/` folder
- [x] Create `legacy/Entities/` folder
- [x] Create `legacy/Models/` folder
- [x] Create `docs/` folder
- [x] Create `data/` folder

### Phase 2: Move Legacy Files
- [ ] Move old Controllers to legacy/
- [ ] Move Views to legacy/
- [ ] Move Scripts to legacy/
- [ ] Move Content to legacy/
- [ ] Move Areas to legacy/
- [ ] Move App_Start to legacy/
- [ ] Move DeviceService.asmx to legacy/
- [ ] Move Processor/ to legacy/
- [ ] Move Entity/ to legacy/
- [ ] Move Models/ to legacy/
- [ ] Move Global.asax to legacy/

### Phase 3: Move Documentation
- [ ] Move all .md files except .gitignore to docs/

### Phase 4: Clean Build Artifacts
- [ ] Remove bin/, obj/ from root
- [ ] Remove compiled DLLs
- [ ] Keep data/ for runtime data

### Phase 5: Update Configuration
- [ ] Update .gitignore
- [ ] Verify Services.sln still works
- [ ] Test build

## Benefits

1. **Clear Separation**: Modern vs Legacy code clearly separated
2. **Cleaner Root**: Only essential files in root
3. **Better Organization**: Logical grouping
4. **Easier Navigation**: Developers know where to look
5. **Preserved History**: Legacy code archived, not deleted
6. **Scalable Structure**: Ready for growth

## Migration Path

Legacy code is preserved in `legacy/` folder for reference during migration. New development happens in `src/` with Clean Architecture.

