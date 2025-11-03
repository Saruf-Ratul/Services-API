# Security Best Practices

## 🔐 Secret Management

This repository uses **placeholder values** for all sensitive configuration. Never commit real secrets to Git.

### Replaced Secrets

All sensitive values have been replaced with placeholders:

- **Database Passwords**: `YOUR_DB_PASSWORD`
- **JWT Secret Keys**: `YOUR_SUPER_SECRET_JWT_KEY_AT_LEAST_32_CHARACTERS_LONG`
- **Twilio Account SID**: `YOUR_TWILIO_ACCOUNT_SID`
- **Twilio Auth Token**: `YOUR_TWILIO_AUTH_TOKEN`
- **Twilio API Key**: `YOUR_TWILIO_API_KEY`
- **Email Passwords**: `YOUR_EMAIL_PASSWORD`
- **OIDC Client Secrets**: `your-client-secret`

### Configuration Files

#### For Development
Copy `appsettings.Example.json` to `appsettings.Development.json` and fill in your values:
```bash
cp src/Services.API/appsettings.Example.json src/Services.API/appsettings.Development.json
```

#### For Production
Copy `appsettings.Example.json` to `appsettings.Production.json` and fill in production values:
```bash
cp src/Services.API/appsettings.Example.json src/Services.API/appsettings.Production.json
```

### Ignored Files

The following files are ignored by Git (via `.gitignore`):
- `appsettings.Development.json`
- `appsettings.Production.json`
- `publish/**/appsettings*.json`
- `publish/**/web.config`
- `.env` files

### Recommended Approach

1. **Use Environment Variables** (Recommended)
   ```bash
   export ConnectionStrings__DefaultConnection="your-connection-string"
   export JwtSettings__SecretKey="your-jwt-secret"
   ```

2. **Use User Secrets** (Development)
   ```bash
   dotnet user-secrets init
   dotnet user-secrets set "JwtSettings:SecretKey" "your-secret"
   ```

3. **Use Azure Key Vault** (Production)
   - Store secrets in Azure Key Vault
   - Configure Key Vault in `Program.cs`

4. **Use Configuration Files** (Local/IIS)
   - Create `appsettings.Development.json` or `appsettings.Production.json`
   - These files are already in `.gitignore`
   - Never commit them to Git

---

## 🚨 If Secrets Were Committed

If you accidentally committed secrets to Git:

### Step 1: Remove from Current Files ✅ DONE
All secrets have been replaced with placeholders.

### Step 2: Clean Git History (if secrets were pushed)

**Option A: Reset History (if you haven't pushed publicly)**
```bash
# Remove the commit with secrets
git reset --hard HEAD~1  # Adjust as needed
git push --force origin main
```

**Option B: Use BFG Repo-Cleaner (recommended for public repos)**
```bash
# Install BFG: https://rtyley.github.io/bfg-repo-cleaner/
# Replace secrets in history
java -jar bfg.jar --replace-text secrets.txt
git reflog expire --expire=now --all
git gc --prune=now --aggressive
git push --force origin main
```

**Option C: Create New Repository (simplest)**
- Create a fresh repository
- Copy files (excluding secrets)
- Update remote URL

---

## 🔒 Security Checklist

Before committing:
- [ ] No real passwords in code
- [ ] No API keys in code
- [ ] No connection strings with passwords
- [ ] No JWT secrets
- [ ] No OAuth client secrets
- [ ] Sensitive files in `.gitignore`
- [ ] Using placeholders in example files

---

## 📝 Configuration Template

Use `appsettings.Example.json` as a template. It contains all configuration sections with placeholder values.

---

## 🆘 GitHub Secret Scanning

GitHub automatically scans for secrets. If you see warnings:
1. Review the detected secrets
2. Rotate compromised credentials immediately
3. Remove secrets from Git history
4. Update `.gitignore` if needed

---

**Remember**: Secrets in Git history are **permanent** unless you clean the history. Always use placeholders in committed files!

