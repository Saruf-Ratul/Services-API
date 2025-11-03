# IIS Deployment Instructions for Services API

## 📋 Prerequisites

### On Windows Server:
1. **.NET 9 Hosting Bundle** installed
   - Download from: https://dotnet.microsoft.com/download/dotnet/9.0
   - Install: `dotnet-hosting-9.0.x-win.exe`
   - Restart IIS after installation

2. **IIS Features Enabled:**
   - ASP.NET Core Module V2
   - Static Content
   - Default Document
   - Directory Browsing (optional)

3. **Application Pool:**
   - .NET CLR Version: **No Managed Code**
   - Managed Pipeline Mode: **Integrated**

---

## 🚀 Deployment Steps

### Step 1: Copy Files to Server

1. Copy entire contents of `publish/IIS/` folder to your server
2. Recommended location: `C:\inetpub\ServicesAPI\`

### Step 2: Create IIS Application Pool

1. Open **IIS Manager**
2. Right-click **Application Pools** → **Add Application Pool**
3. Configure:
   - **Name**: `ServicesAPI`
   - **.NET CLR Version**: **No Managed Code**
   - **Managed Pipeline Mode**: **Integrated**
4. Click **OK**

### Step 3: Configure Application Pool

1. Select **ServicesAPI** application pool
2. Click **Advanced Settings**
3. Set:
   - **Identity**: `ApplicationPoolIdentity` (or custom account with DB permissions)
   - **Start Mode**: `AlwaysRunning` (optional, for better performance)
   - **Idle Timeout**: `0` (or desired timeout)

### Step 4: Create IIS Website/Application

**Option A: Create New Website**
1. Right-click **Sites** → **Add Website**
2. Configure:
   - **Site name**: `ServicesAPI`
   - **Application pool**: `ServicesAPI`
   - **Physical path**: `C:\inetpub\ServicesAPI` (or your path)
   - **Binding**:
     - **Type**: `https` or `http`
     - **IP address**: `All Unassigned` or specific IP
     - **Port**: `443` (HTTPS) or `80` (HTTP)
     - **Host name**: Your domain (e.g., `api.yourcompany.com`)
3. Click **OK**

**Option B: Add as Application (if using existing site)**
1. Right-click your existing site → **Add Application**
2. Configure:
   - **Alias**: `ServicesAPI`
   - **Application pool**: `ServicesAPI`
   - **Physical path**: `C:\inetpub\ServicesAPI`
3. Click **OK**

### Step 5: Configure Permissions

1. Right-click `C:\inetpub\ServicesAPI` folder → **Properties**
2. **Security** tab → **Edit**
3. Add **IIS_IUSRS** with **Read & Execute** permissions
4. Add **Application Pool Identity** (e.g., `IIS AppPool\ServicesAPI`) with **Read & Execute**
5. Ensure **IIS_IUSRS** has **Write** permissions to `logs/` folder (if it exists)

### Step 6: Configure SSL Certificate (if using HTTPS)

1. Select your site in IIS Manager
2. Click **Bindings**
3. Click **Add** or **Edit**
4. Configure:
   - **Type**: `https`
   - **SSL certificate**: Select your certificate
5. Click **OK**

---

## ⚙️ Configuration

### Update appsettings.Production.json

Before deployment, update:
- `ConnectionStrings` - Your production database
- `JwtSettings` - Production JWT keys
- `OidcSettings` - Production OIDC provider
- `AllowedOrigins` - Production frontend URLs

### Environment Variables

Ensure `ASPNETCORE_ENVIRONMENT=Production` in web.config (already configured)

### Logs Folder

1. Create folder: `C:\inetpub\ServicesAPI\logs`
2. Give IIS_IUSRS **Write** permissions
3. Logs will be written to: `logs\log-YYYYMMDD.txt`

---

## 🔍 Verification

### 1. Test Health Endpoint
```
https://your-domain/health
```
Expected: JSON response with health status

### 2. Test Swagger
```
https://your-domain/swagger
```
Expected: Swagger UI interface

### 3. Check Application Pool Status
- Open IIS Manager
- Check **Application Pools** → **ServicesAPI**
- Status should be **Started**

### 4. Check Event Logs
- Windows Event Viewer → **Application** log
- Look for ASP.NET Core entries

---

## 🛠️ Troubleshooting

### Problem: 500.0 Error (ANCM In-Process Handler Load Failure)
**Solution:**
- Install .NET 9 Hosting Bundle
- Restart IIS: `iisreset`
- Verify `web.config` is present

### Problem: 502.5 Process Failure
**Solution:**
- Check application pool identity has permissions
- Verify database connection string
- Check `stdout` logs in `logs/` folder

### Problem: Swagger Not Loading
**Solution:**
- Enable Swagger in production: `appsettings.Production.json`
- Check `SwaggerSettings:EnableSwaggerUI = true`
- Verify HTTPS/HTTP binding

### Problem: Database Connection Failed
**Solution:**
- Verify SQL Server is accessible
- Check connection string in `appsettings.Production.json`
- Ensure Application Pool identity has DB permissions
- Check firewall rules

### Problem: Permission Denied
**Solution:**
- Grant IIS_IUSRS Read/Execute on application folder
- Grant Application Pool identity Write on `logs/` folder
- Verify `web.config` is accessible

---

## 📊 Performance Optimization

### Enable Compression
Already configured in `Program.cs` - should work automatically

### Enable Static Content Caching
Add to `web.config`:
```xml
<staticContent>
  <clientCache cacheControlMode="UseMaxAge" cacheControlMaxAge="7.00:00:00" />
</staticContent>
```

### Application Pool Recycling
- Set appropriate recycling schedule
- Consider `AlwaysRunning` for better startup performance

---

## 🔐 Security Checklist

- [ ] HTTPS enabled with valid certificate
- [ ] Production connection strings configured
- [ ] JWT secret key changed from default
- [ ] OIDC settings configured
- [ ] CORS origins restricted to production domains
- [ ] Rate limiting enabled
- [ ] Security headers enabled
- [ ] Error pages configured (hide detailed errors)
- [ ] `appsettings.json` not accessible via HTTP

---

## 📝 Post-Deployment

### 1. Monitor Logs
- Check `logs/` folder regularly
- Monitor Windows Event Viewer

### 2. Test All Endpoints
- Use Swagger UI to test
- Verify authentication works
- Test critical business flows

### 3. Set Up Monitoring
- Configure health check monitoring
- Set up alerts for 500 errors
- Monitor application pool

---

## 🎯 Quick Reference

| Item | Location |
|------|----------|
| Application Files | `C:\inetpub\ServicesAPI\` |
| Logs | `C:\inetpub\ServicesAPI\logs\` |
| Config | `appsettings.Production.json` |
| Web Config | `web.config` |
| Health Check | `/health` |
| Swagger UI | `/swagger` |

---

## ✅ Deployment Complete!

Your Services API should now be running on IIS!

**Access:**
- **Swagger**: `https://your-domain/swagger`
- **Health**: `https://your-domain/health`
- **API**: `https://your-domain/api/{endpoint}`

---

**Created**: 2025-01-27  
**Version**: 1.0

