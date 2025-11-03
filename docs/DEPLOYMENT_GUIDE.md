# Deployment Guide - Services API

## Table of Contents
1. [Development Setup](#development-setup)
2. [Local Testing](#local-testing)
3. [IIS Deployment](#iis-deployment)
4. [Docker Deployment](#docker-deployment)
5. [Kubernetes Deployment](#kubernetes-deployment)
6. [Security Checklist](#security-checklist)
7. [Troubleshooting](#troubleshooting)

---

## Development Setup

### Prerequisites
- .NET 9 SDK
- Visual Studio 2022 or VS Code
- SQL Server access
- Redis (Docker or local)
- Git

### Initial Setup

1. **Clone and Restore**
   ```bash
   cd D:\XSI\Services\Services
   dotnet restore Services.sln
   ```

2. **Start Redis**
   ```bash
   docker run -d -p 6379:6379 --name services-redis redis:7-alpine
   ```

3. **Update Connection Strings**
   - Edit `src/Services.API/appsettings.Development.json`
   - Update database connection strings
   - Update Redis connection string if needed

4. **Build Solution**
   ```bash
   dotnet build Services.sln -c Debug
   ```

5. **Run Application**
   ```bash
   cd src/Services.API
   dotnet run
   ```

6. **Access Swagger UI**
   - Open browser: `https://localhost:7148/swagger`
   - Or: `http://localhost:5148/swagger` (HTTP)

---

## Local Testing

### Test Endpoints

1. **Health Check**
   ```bash
   curl https://localhost:7148/health
   ```

2. **Swagger UI**
   ```bash
   # Open in browser
   https://localhost:7148/swagger
   ```

3. **Test with Postman**
   - Import OpenAPI spec from Swagger
   - Use Bearer token authentication
   - Test all endpoints

### Test Authentication

1. **Login Endpoint**
   ```bash
   POST https://localhost:7148/api/auth/login
   Content-Type: application/json
   
   {
     "username": "testuser",
     "password": "testpass"
   }
   ```

2. **Use Token**
   ```bash
   GET https://localhost:7148/api/customers
   Authorization: Bearer YOUR_TOKEN_HERE
   ```

---

## IIS Deployment

### Requirements
- Windows Server with IIS 10+
- ASP.NET Core Hosting Bundle 9.0
- SQL Server access
- Redis server access

### Step 1: Install Prerequisites

1. **Install .NET 9 Hosting Bundle**
   ```powershell
   # Download from: https://dotnet.microsoft.com/download/dotnet/9.0
   # Run installer as Administrator
   ```

2. **Install IIS Features**
   ```powershell
   Enable-WindowsOptionalFeature -Online -FeatureName IIS-WebServerRole
   Enable-WindowsOptionalFeature -Online -FeatureName IIS-WebServer
   Enable-WindowsOptionalFeature -Online -FeatureName IIS-CommonHttpFeatures
   Enable-WindowsOptionalFeature -Online -FeatureName IIS-HttpErrors
   Enable-WindowsOptionalFeature -Online -FeatureName IIS-ApplicationInit
   ```

3. **Restart IIS**
   ```powershell
   iisreset
   ```

### Step 2: Publish Application

1. **Build Release Version**
   ```bash
   dotnet publish Services.sln -c Release -o ./publish
   ```

2. **Copy Files to Server**
   - Copy `publish` folder to: `C:\inetpub\servicesapi\`
   - Ensure permissions are set correctly

### Step 3: Configure IIS

1. **Create Application Pool**
   - Open IIS Manager
   - Right-click "Application Pools" → "Add Application Pool"
   - Name: `ServicesAPIPool`
   - .NET CLR Version: **No Managed Code**
   - Managed Pipeline Mode: **Integrated**
   - Identity: **ApplicationPoolIdentity**
   - Click OK

2. **Create Website**
   - Right-click "Sites" → "Add Website"
   - Site Name: `ServicesAPI`
   - Application Pool: `ServicesAPIPool`
   - Physical Path: `C:\inetpub\servicesapi`
   - Binding: HTTP (80) or HTTPS (443 with certificate)

3. **Configure Advanced Settings**
   - Preload Enabled: **True**
   - Set proper permissions for IIS_IUSRS

### Step 4: Configure appsettings.Production.json

Edit `src/Services.API/appsettings.Production.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=Mobilize;User Id=YOUR_USER;Password=YOUR_PASS;TrustServerCertificate=true",
    "Redis": "YOUR_REDIS_SERVER:6379"
  },
  "JwtSettings": {
    "SecretKey": "PRODUCTION_SECRET_KEY_MIN_32_CHARACTERS",
    "Issuer": "ServicesAPI-Production",
    "Audience": "ServicesAPIUsers-Production"
  },
  "AllowedOrigins": [
    "https://yourdomain.com",
    "https://www.yourdomain.com"
  ]
}
```

### Step 5: SSL Certificate

1. **Install Certificate**
   - Import SSL certificate to "Server Certificates"
   - Bind to port 443
   - Force HTTPS redirect

2. **web.config Auto-Generated**
   - ASP.NET Core creates web.config automatically
   - Or use provided web.config template

### Step 6: Firewall Configuration

1. **Allow Ports**
   ```powershell
   New-NetFirewallRule -DisplayName "Services API HTTP" -Direction Inbound -Protocol TCP -LocalPort 80
   New-NetFirewallRule -DisplayName "Services API HTTPS" -Direction Inbound -Protocol TCP -LocalPort 443
   ```

2. **Test Connection**
   ```bash
   curl https://yourdomain.com/health
   ```

---

## Docker Deployment

### Step 1: Build Docker Image

```bash
cd infrastructure/docker
docker build -t services-api:latest .
```

Or build from solution root:

```bash
docker build -f infrastructure/docker/Dockerfile -t services-api:latest .
```

### Step 2: Run with Docker Compose

```bash
cd infrastructure/docker

# Create .env file with your settings
cat > .env << EOF
DB_CONNECTION_STRING=Server=your-db;Database=Mobilize;User Id=user;Password=pass;TrustServerCertificate=true
SA_PASSWORD=YourStrong@Passw0rd
EOF

docker-compose up -d
```

### Step 3: Verify

```bash
# Check logs
docker-compose logs -f api

# Check health
curl http://localhost:8080/health

# Check Swagger
curl http://localhost:8080/swagger
```

### Step 4: Production Docker Deployment

1. **Push to Registry**
   ```bash
   docker tag services-api:latest yourregistry.azurecr.io/services-api:latest
   docker push yourregistry.azurecr.io/services-api:latest
   ```

2. **Pull on Server**
   ```bash
   docker pull yourregistry.azurecr.io/services-api:latest
   docker run -d -p 80:80 -p 443:443 services-api:latest
   ```

---

## Kubernetes Deployment

### Prerequisites
- Kubernetes cluster (AKS, EKS, GKE, or on-prem)
- kubectl configured
- Helm (optional)

### Step 1: Create Namespace

```bash
kubectl create namespace services
```

### Step 2: Create Secrets

```bash
# Database secret
kubectl create secret generic services-db-secrets \
  --from-literal=connection-string='Server=your-server;Database=Mobilize;User Id=user;Password=pass' \
  -n services

# JWT secret
kubectl create secret generic services-jwt-secrets \
  --from-literal=secret-key='YOUR_SECRET_KEY_MIN_32_CHARACTERS' \
  -n services

# OIDC secrets
kubectl create secret generic services-oidc-secrets \
  --from-literal=client-id='your-client-id' \
  --from-literal=client-secret='your-client-secret' \
  -n services

# Redis secret
kubectl create secret generic services-redis-secrets \
  --from-literal=connection-string='redis-service:6379' \
  -n services
```

### Step 3: Deploy Application

```bash
# Apply all resources
kubectl apply -f infrastructure/kubernetes/deployment.yaml

# Verify deployment
kubectl get pods -n services
kubectl get services -n services
```

### Step 4: Check Logs

```bash
# Get pod name
kubectl get pods -n services

# View logs
kubectl logs -f <pod-name> -n services
```

### Step 5: Configure Ingress

```bash
# If using ingress controller
kubectl apply -f infrastructure/kubernetes/ingress.yaml

# Update DNS to point to ingress IP
```

### Step 6: Monitor

```bash
# Watch health checks
kubectl get pods -n services -w

# Describe pod for events
kubectl describe pod <pod-name> -n services
```

---

## Terraform Deployment (Azure)

### Step 1: Initialize Terraform

```bash
cd infrastructure/terraform
terraform init
```

### Step 2: Create terraform.tfvars

```hcl
resource_group_name = "services-rg-prod"
location            = "East US"
cluster_name        = "services-aks-prod"
sql_admin_login     = "sqladmin"
sql_admin_password  = "YourStrong@Passw0rd!123"
```

### Step 3: Plan Deployment

```bash
terraform plan -var-file="terraform.tfvars"
```

### Step 4: Apply Configuration

```bash
terraform apply -var-file="terraform.tfvars"
```

### Step 5: Get Outputs

```bash
terraform output kubernetes_cluster_name
terraform output kubernetes_cluster_fqdn
terraform output redis_hostname
terraform output sql_server_fqdn
```

### Step 6: Configure kubectl

```bash
az aks get-credentials --resource-group services-rg-prod --name services-aks-prod
```

---

## Security Checklist

### Pre-Deployment

- [ ] Update all connection strings in appsettings
- [ ] Change JWT secret key to strong random value (min 32 characters)
- [ ] Configure OIDC authority URLs
- [ ] Update AllowedOrigins with production domains
- [ ] Enable HTTPS only
- [ ] Configure SSL certificates
- [ ] Set up firewall rules
- [ ] Enable rate limiting
- [ ] Configure security headers
- [ ] Disable debug mode in production
- [ ] Remove development-only features
- [ ] Review and update Swagger settings

### Post-Deployment

- [ ] Verify HTTPS redirect working
- [ ] Test health check endpoint
- [ ] Test authentication flow
- [ ] Test rate limiting (should 429 after limit)
- [ ] Verify CORS headers
- [ ] Check security headers present
- [ ] Monitor logs for errors
- [ ] Set up monitoring/alerting
- [ ] Configure backup strategy
- [ ] Document credentials securely

### Ongoing

- [ ] Regular security updates
- [ ] Monitor for vulnerabilities
- [ ] Review access logs
- [ ] Update dependencies regularly
- [ ] Rotate secrets periodically
- [ ] Keep SSL certificates valid

---

## Troubleshooting

### Issue: Application won't start

**Check**:
```bash
# Windows Event Log
eventvwr.msc

# IIS Logs
C:\inetpub\logs\LogFiles\

# Application Logs
cat logs/log-*.txt

# Docker Logs
docker logs services-api
```

**Common Causes**:
- Connection string incorrect
- Port already in use
- Missing dependencies
- Permission issues

**Fix**:
```bash
# Check if port is in use
netstat -ano | findstr :8080

# Test database connection
sqlcmd -S your-server -U your-user -P your-pass -Q "SELECT 1"

# Verify Redis
redis-cli -h localhost ping
```

### Issue: Swagger not loading

**Check**:
- `SwaggerSettings:EnableSwaggerUI = true` in appsettings
- Not in production with authentication required
- HTTPS requirement blocking HTTP

**Fix**:
```json
{
  "SwaggerSettings": {
    "EnableSwaggerUI": true,
    "RequireAuthentication": false
  }
}
```

### Issue: Authentication failing

**Check**:
- JWT secret key configured
- OIDC authority accessible
- MFA not required for test user
- Clock skew (system time sync)

**Fix**:
```bash
# Test JWT locally
dotnet user-secrets set "JwtSettings:SecretKey" "your-secret"

# Check system time
date

# Verify OIDC endpoint
curl https://your-identity-provider.com/.well-known/openid-configuration
```

### Issue: Database connection fails

**Check**:
- Server accessible from application server
- Firewall allows SQL Server port (1433)
- Connection string has correct credentials
- Database exists and accessible

**Fix**:
```bash
# Test connection
sqlcmd -S 172.168.90.16 -U Mobilizedba -P Mobilizedba -Q "SELECT 1"

# Check firewall
netsh advfirewall firewall show rule name="SQL Server"

# Add firewall rule
netsh advfirewall firewall add rule name="SQL Server" dir=in action=allow protocol=TCP localport=1433
```

### Issue: Redis connection fails

**Check**:
- Redis server running
- Port 6379 accessible
- Network connectivity
- Firewall rules

**Fix**:
```bash
# Test Redis locally
redis-cli -h localhost ping

# Check Redis in Docker
docker ps | grep redis
docker logs services-redis

# Test from application server
telnet redis-server 6379
```

### Issue: High memory usage

**Check**:
- Memory leaks in application
- Caching too much data
- Too many connections

**Fix**:
- Implement proper disposal
- Configure cache size limits
- Use connection pooling
- Monitor with application insights

### Issue: Slow performance

**Check**:
- Database query performance
- Network latency
- Redis cache hit rate
- Application bottlenecks

**Fix**:
- Add database indexes
- Optimize queries
- Increase cache TTL
- Scale horizontally
- Add CDN for static content

---

## Monitoring

### Application Insights

```bash
# Add to appsettings
{
  "ApplicationInsights": {
    "InstrumentationKey": "your-key"
  }
}
```

### Health Checks UI

Access: `https://yourdomain.com/health-ui`

Shows:
- Database connectivity
- Redis connectivity
- Custom health checks

### Logs

**Locations**:
- Windows: `C:\inetpub\servicesapi\logs\`
- Docker: `docker logs services-api`
- Kubernetes: `kubectl logs -f <pod> -n services`

---

## Rollback Procedures

### IIS Rollback

```powershell
# Stop site
Stop-Website -Name ServicesAPI

# Copy old version
Copy-Item C:\Backup\servicesapi-old C:\inetpub\servicesapi -Recurse -Force

# Start site
Start-Website -Name ServicesAPI
```

### Docker Rollback

```bash
# Stop current container
docker stop services-api

# Run previous version
docker run -d -p 8080:80 services-api:previous-version
```

### Kubernetes Rollback

```bash
# Rollback deployment
kubectl rollout undo deployment/services-api -n services

# Check status
kubectl rollout status deployment/services-api -n services
```

---

## Support Contacts

- **Developer Team**: dev@servicesapi.com
- **DevOps Team**: devops@servicesapi.com
- **Security Team**: security@servicesapi.com

## Additional Resources

- [ASP.NET Core Deployment](https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/)
- [IIS Configuration](https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/iis/)
- [Docker Documentation](https://docs.docker.com/)
- [Kubernetes Documentation](https://kubernetes.io/docs/)
- [Terraform Documentation](https://www.terraform.io/docs/)

---

**Last Updated**: 2025-01-27  
**Version**: 1.0

