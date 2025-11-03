# 🚀 How to Run Services API

## Quick Start

### Method 1: Using PowerShell Script (Recommended)
```powershell
cd D:\XSI\Services\Services
.\run.ps1
```

### Method 2: Using dotnet CLI
```powershell
cd D:\XSI\Services\Services
dotnet run --project src/Services.API
```

### Method 3: Using Visual Studio
1. Open `Services.sln` in Visual Studio
2. Set `Services.API` as startup project
3. Press F5 or click Run

---

## 🌐 Access Points

Once the API is running, access:

| Service | URL | Description |
|---------|-----|-------------|
| **Swagger UI** | https://localhost:7148/swagger | API Documentation |
| **HTTP Swagger** | http://localhost:5148/swagger | HTTP version |
| **Health Check** | http://localhost:5148/health | Health status |
| **Health UI** | http://localhost:5148/health-ui | Health dashboard |

---

## 🔍 Verify It's Running

### Check Process
```powershell
Get-Process dotnet
```

### Test Health Endpoint
```powershell
Invoke-WebRequest -Uri "http://localhost:5148/health"
```

### Check Listening Ports
```powershell
Get-NetTCPConnection -State Listen | Where-Object {$_.LocalPort -in 5148,7148}
```

---

## ⚠️ Troubleshooting

### Problem: Port Already in Use
```powershell
# Find and kill process using port
Get-Process -Id (Get-NetTCPConnection -LocalPort 7148).OwningProcess | Stop-Process
```

### Problem: Build Errors
```powershell
# Clean and rebuild
dotnet clean Services.sln
dotnet restore Services.sln
dotnet build Services.sln
```

### Problem: Missing Dependencies
```powershell
# Restore packages
dotnet restore Services.sln
```

---

## 📝 Configuration

### Development
Uses `appsettings.Development.json`
- Swagger enabled
- Detailed logging
- Debug mode

### Production
Uses `appsettings.Production.json`
- Reduced logging
- Performance optimizations
- Security hardening

---

## 🔐 Authentication

### Test with Swagger
1. Open https://localhost:7148/swagger
2. Click "Authorize" button
3. Enter JWT token or use OAuth2

### Test with curl
```bash
curl http://localhost:5148/api/customers \
  -H "Authorization: Bearer YOUR_TOKEN"
```

---

## 📊 Monitor Health

### Health Checks
- `/health` - Basic health status
- `/health-ui` - Detailed dashboard

### Logs
- Console output
- File: `logs/log-YYYYMMDD.txt`

---

## 🛑 Stop the Server

### Method 1: In Terminal
Press `Ctrl+C`

### Method 2: Kill Process
```powershell
Get-Process dotnet | Stop-Process -Force
```

---

## ✅ Success Indicators

You'll know it's running when you see:
```
✅ Build successful
🌐 Now listening on: https://localhost:7148
🌐 Now listening on: http://localhost:5148
```

---

## 📖 Next Steps

Once running:
1. Open Swagger UI
2. Explore endpoints
3. Test with "Try it out"
4. Check health status
5. Review logs

For more details, see `docs/SETUP_INSTRUCTIONS.md`

---

**Happy Coding! 🎉**

