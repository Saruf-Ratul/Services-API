# Services API - IIS Deployment Package

## 📦 Package Contents

This folder contains **all files needed** to deploy Services API to Windows Server IIS.

### Critical Files:
- ✅ `Services.API.dll` - Main application
- ✅ `Services.API.exe` - Entry point
- ✅ `web.config` - IIS configuration
- ✅ `appsettings.json` - Configuration
- ✅ `appsettings.Production.json` - Production settings
- ✅ All required DLLs and dependencies

### Size: ~133 MB

---

## 🚀 Quick Deployment

### 1. Copy This Folder
Copy entire `IIS` folder to Windows Server:
```
C:\inetpub\ServicesAPI\
```

### 2. Install Prerequisites
- Install **.NET 9 Hosting Bundle** on server
- Restart IIS after installation

### 3. Configure IIS
- Create Application Pool: **No Managed Code**, **Integrated**
- Create Website/Application pointing to folder
- Set permissions (see DEPLOYMENT_INSTRUCTIONS.md)

### 4. Configure Settings
- Edit `appsettings.Production.json`
- Update connection strings
- Configure JWT/OIDC settings

### 5. Test
- Access: `https://your-domain/swagger`
- Check: `https://your-domain/health`

---

## 📚 Full Instructions

See: **DEPLOYMENT_INSTRUCTIONS.md** for detailed steps

---

## ✅ Pre-Deployment Checklist

Before deploying:

- [ ] Update `appsettings.Production.json` with production values
- [ ] Verify database connection strings
- [ ] Configure JWT secret keys
- [ ] Set OIDC/OAuth2 provider URLs
- [ ] Update `AllowedOrigins` with production domains
- [ ] Review security settings
- [ ] Test locally first

---

## 🔍 Verification

After deployment, verify:

1. **Health Check**: `/health` returns 200
2. **Swagger UI**: `/swagger` loads correctly
3. **API Endpoints**: Test a few endpoints
4. **Logs**: Check `logs/` folder for errors
5. **Application Pool**: Status is "Started"

---

## 📞 Support

For issues, check:
- `DEPLOYMENT_INSTRUCTIONS.md` - Troubleshooting section
- Windows Event Viewer - Application logs
- `logs/` folder - Application logs

---

**Version**: 1.0  
**Build Date**: 2025-01-27  
**Target**: Windows Server IIS  
**.NET**: 9.0

