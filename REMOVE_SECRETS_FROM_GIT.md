# Remove Secrets from Git History

## ⚠️ Important Notice

Secrets were detected in your Git history by GitHub. This guide helps you remove them.

## ✅ Step 1: Secrets Already Replaced in Current Files

All secrets in current files have been replaced with placeholders:
- ✅ `src/Services.API/appsettings.json` - Updated
- ✅ `docs/FINAL_STATUS.md` - Updated  
- ✅ `legacy/OldWebForms/Web.config` - Updated

## 🔧 Step 2: Clean Git History

Since secrets were already pushed, you need to remove them from Git history.

### Option A: Force Push (If repository is private/new)

**⚠️ Only use if:**
- Repository is private, OR
- Very few people have cloned it, OR
- You're okay with rewriting history

```powershell
# Check commit history
git log --oneline

# Find the commit with secrets (likely the first commit)
# Reset to before that commit (or delete it)
git rebase -i HEAD~<number-of-commits>

# Or reset completely and recommit
git reset --soft <commit-before-secrets>
git add .
git commit -m "Initial commit - Clean version without secrets"
git push origin main --force
```

### Option B: Use BFG Repo-Cleaner (Recommended)

BFG is a faster, simpler alternative to `git filter-branch`:

```powershell
# 1. Install Java (required for BFG)
# Download: https://adoptium.net/

# 2. Download BFG
# Download: https://rtyley.github.io/bfg-repo-cleaner/
# Save as: bfg.jar

# 3. Create a file with secrets to replace
# Create secrets.txt (replace with YOUR actual secrets that were exposed):
# Format: old_secret==>replacement_value
# Example:
# echo "AC[YOUR_OLD_ACCOUNT_SID]==>YOUR_TWILIO_ACCOUNT_SID" > secrets.txt
# echo "SK[YOUR_OLD_API_KEY]==>YOUR_TWILIO_API_KEY" >> secrets.txt
# echo "[YOUR_OLD_AUTH_TOKEN]==>YOUR_TWILIO_AUTH_TOKEN" >> secrets.txt

# 4. Clone a fresh copy (BFG needs this)
git clone --mirror https://github.com/Saruf-Ratul/Services-API.git

# 5. Run BFG
java -jar bfg.jar --replace-text secrets.txt Services-API.git

# 6. Clean up and force push
cd Services-API.git
git reflog expire --expire=now --all
git gc --prune=now --aggressive
git push origin --force --all

# 7. Delete the mirror and update your local repo
cd ..
rmdir /s /q Services-API.git
cd Services-API
git fetch origin
git reset --hard origin/main
```

### Option C: Use Git Filter-Repo (Modern Alternative)

```powershell
# Install git-filter-repo (Python)
pip install git-filter-repo

# Remove secrets from history
git filter-repo --replace-text secrets.txt
git push origin --force --all
```

### Option D: Create New Repository (Simplest)

If the repository is new and you haven't shared it widely:

```powershell
# 1. Create new repository on GitHub

# 2. Remove old remote
git remote remove origin

# 3. Add new remote
git remote add origin https://github.com/Saruf-Ratul/Services-API-NEW.git

# 4. Verify files don't have secrets (already done)
# 5. Commit current state
git add .
git commit -m "Initial commit - Clean version"
git push -u origin main
```

## 🔄 Step 3: After Cleaning History

### For All Collaborators

If others have cloned the repository, they need to:

```powershell
# Fetch latest
git fetch origin

# Reset their local branch
git reset --hard origin/main

# Clean up
git gc --prune=now
```

### Rotate Compromised Credentials

**⚠️ CRITICAL**: Since secrets were exposed in Git history, rotate ALL affected credentials:

1. **Twilio**: 
   - Generate new Account SID
   - Generate new Auth Token
   - Generate new API Key
   - Update in your actual config files

2. **Database Passwords**: 
   - Change all database passwords
   - Update connection strings

3. **Email Passwords**: 
   - Change email service passwords
   - Update SMTP credentials

4. **JWT Secrets**: 
   - Generate new JWT secret keys
   - Update in all environments

## ✅ Step 4: Verify

1. Check that GitHub no longer shows secret warnings
2. Verify all files use placeholders:
   ```powershell
   # Should show no real secrets
   git grep -i "AC[a-z0-9]\{32\}" 
   git grep -i "SK[a-z0-9]\{32\}"
   ```
3. Test that repository pushes successfully
4. Check GitHub Security tab for any remaining alerts

## 📚 Resources

- [GitHub: Removing sensitive data](https://docs.github.com/en/authentication/keeping-your-account-and-data-secure/removing-sensitive-data-from-a-repository)
- [BFG Repo-Cleaner](https://rtyley.github.io/bfg-repo-cleaner/)
- [Git Filter-Repo](https://github.com/newren/git-filter-repo)

---

## 🎯 Quick Commands Summary

```powershell
# Check current secrets (should show none)
# Replace with your actual secret patterns:
# git grep "AC[a-f0-9]\{32\}"
# git grep "SK[a-f0-9]\{32\}"

# If you see results, files still need updating
# If no results, secrets are replaced in current files

# Verify .gitignore
git check-ignore -v appsettings.Development.json
git check-ignore -v appsettings.Production.json
```

---

**Next Steps**: 
1. Choose one of the options above to clean Git history
2. Rotate all compromised credentials
3. Verify GitHub no longer shows warnings
4. Continue with normal Git workflow using placeholders

