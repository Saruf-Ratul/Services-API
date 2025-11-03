# ✅ IIS Deployment Package Created Successfully!

## 🎉 **Ready for Windows Server IIS Deployment**

Your Services API has been **published** and is ready for IIS deployment!

---

## 📦 **Published Package**

### Location
```
D:\XSI\Services\Services\publish\IIS\
```

### Package Contents
- ✅ **115 files** published
- ✅ **132.71 MB** total size
- ✅ All required DLLs and dependencies
- ✅ Configuration files
- ✅ web.config for IIS
- ✅ Deployment documentation

### Critical Files Included:
- `Services.API.dll` - Main application
- `Services.API.exe` - Entry point
- `web.config` - IIS configuration
- `appsettings.json` - Base configuration
- `appsettings.Production.json` - Production settings
- All NuGet package dependencies

---

## 🚀 **Deployment Steps**

### Step 1: Copy to Server
Copy the entire `publish\IIS\` folder to your Windows Server:
```
Recommended: C:\inetpub\ServicesAPI\
```

### Step 2: Install Prerequisites

**On Windows Server:**
1. Download **.NET 9 Hosting Bundle**
   - URL: https://dotnet.microsoft.com/download/dotnet/9.0
   - File: `dotnet-hosting-9.0.x-win.exe`
2. Install the bundle
3. **Restart IIS**: `iisreset` or restart Windows

### Step 3: Create Application Pool

**In IIS Manager:**
1. Application Pools → Add Application Pool
2. Name: `ServicesAPI`
3. .NET CLR Version: **No Managed Code**
4. Managed Pipeline Mode: **Integrated**
5. Click OK

### Step 4: Create Website/Application

**Option A - New Website:**
1. Sites → Add Website
2. Site name: `ServicesAPI`
3. Application pool: `ServicesAPI`
4. Physical path: `C:\inetpub\ServicesAPI`
5. Binding: Configure HTTPS/HTTP port

**Option B - Add to Existing Site:**
1. Right-click existing site → Add Application
2. Alias: `ServicesAPI`
3. Application pool: `ServicesAPI`
4. Physical path: `C:\inetpub\ServicesAPI`

### Step 5: Set Permissions

**On `C:\inetpub\ServicesAPI` folder:**
1. Properties → Security → Edit
2. Add **IIS_IUSRS**:
   - Read & Execute
   - List folder contents
   - Read
3. Add **Application Pool Identity** (`IIS AppPool\ServicesAPI`):
   - Read & Execute
   - List folder contents
   - Read
4. Create `logs\` folder and give **Write** permission

### Step 6: Configure Production Settings

**Edit `appsettings.Production.json`:**
- Update `ConnectionStrings`
- Set production JWT secret keys
- Configure OIDC/OAuth2 URLs
- Update `AllowedOrigins` with production domains

### Step 7: Test Deployment

**Verify:**
- Health: `https://your-domain/health`
- Swagger: `https://your-domain/swagger`
- API Endpoints: Test a few endpoints

---

## 📁 **Package Structure**

```
publish/IIS/
├── Services.API.dll          # Main application
├── Services.API.exe          # Entry point
├── web.config                # IIS configuration
├── appsettings.json          # Base config
├── appsettings.Production.json # Production config
├── [All DLLs]               # Dependencies
├── wwwroot/                 # Static files
├── README.md                # Package readme
└── DEPLOYMENT_INSTRUCTIONS.md # Full guide
```

---

## ✅ **Pre-Deployment Checklist**

Before deploying to production:

- [ ] **Update Production Config**
  - [ ] Database connection strings
  - [ ] JWT secret keys (change from default!)
  - [ ] OIDC/OAuth2 provider URLs
  - [ ] AllowedOrigins (production domains)
  - [ ] Email settings
  - [ ] Twilio settings

- [ ] **Server Requirements**
  - [ ] .NET 9 Hosting Bundle installed
  - [ ] IIS with ASP.NET Core Module V2
  - [ ] SQL Server accessible
  - [ ] Firewall rules configured

- [ ] **Security**
  - [ ] HTTPS certificate configured
  - [ ] Strong JWT secret keys
  - [ ] CORS origins restricted
  - [ ] Rate limiting enabled

---

## 🔧 **Quick Reference**

### Publish Command
```powershell
.\publish-to-iis.ps1
```

### Manual Publish
```powershell
dotnet publish src/Services.API/Services.API.csproj -c Release -o publish\IIS --self-contained false
```

### IIS Commands
```powershell
# Restart IIS
iisreset

# Check application pool
Get-IISAppPool ServicesAPI

# Test site
Invoke-WebRequest https://your-domain/health
```

---

## 📊 **Package Information**

| Item | Value |
|------|-------|
| **Location** | `publish\IIS\` |
| **Size** | 132.71 MB |
| **Files** | 115 files |
| **Target** | Windows Server IIS |
| **.NET Version** | 9.0 |
| **Framework** | .NET Core (not self-contained) |
| **Configuration** | Release |

---

## 🎯 **After Deployment**

### Access Points:
- **Swagger UI**: `https://your-domain/swagger`
- **Health Check**: `https://your-domain/health`
- **Health Dashboard**: `https://your-domain/health-ui`
- **API Base**: `https://your-domain/api`

### Monitor:
- Check `logs/` folder for application logs
- Windows Event Viewer for IIS errors
- Application Pool status in IIS Manager

---

## 📚 **Documentation**

All deployment guides in `publish/IIS/`:
- `DEPLOYMENT_INSTRUCTIONS.md` - Detailed step-by-step guide
- `README.md` - Package overview

Additional guides in project:
- `docs/DEPLOYMENT_GUIDE.md` - General deployment info
- `docs/SETUP_INSTRUCTIONS.md` - Development setup

---

## 🔄 **Updating Deployment**

When you need to update:

1. Run publish script:
   ```powershell
   .\publish-to-iis.ps1
   ```

2. Copy new files to server (or use IIS deployment tools)

3. Recycle Application Pool:
   ```powershell
   Restart-WebAppPool -Name ServicesAPI
   ```

4. Verify deployment

---

## ⚠️ **Important Notes**

1. **Never deploy `appsettings.Development.json` to production**
2. **Always use HTTPS in production**
3. **Change default JWT secret keys**
4. **Restrict CORS to production domains**
5. **Enable Windows Firewall rules for SQL Server**
6. **Monitor logs regularly**

---

## 🎊 **Success!**

**Your IIS deployment package is ready!**

✅ All files published  
✅ web.config configured  
✅ Documentation included  
✅ Ready for Windows Server  

**Next**: Copy to server and follow `DEPLOYMENT_INSTRUCTIONS.md`

---

**Published**: 2025-01-27  
**Package Location**: `publish\IIS\`  
**Status**: ✅ Ready for Deployment

