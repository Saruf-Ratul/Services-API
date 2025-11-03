# Simple Guide to Clean Git History

GitHub is blocking your push because secrets exist in **old commits** in Git history. Here's the simplest way to fix it:

## ⚠️ Quick Solution: Use GitHub UI

**Easiest way** - Temporarily allow secrets via GitHub UI, then clean history later:

1. Go to these URLs and click "Allow" temporarily:
   - https://github.com/Saruf-Ratul/Services-API/security/secret-scanning/unblock-secret/34xLtnaxCdys61PlLcWljHaEtFm
   - https://github.com/Saruf-Ratul/Services-API/security/secret-scanning/unblock-secret/34xLtne17z8r06LsWaXt432RFd5

2. Then push:
   ```powershell
   git push origin main
   ```

3. **After pushing**, clean history using Option 2 below.

---

## 🔧 Option 2: Clean Git History (Recommended)

Since the repository is new and likely has few/no collaborators, you can safely rewrite history:

### Method A: Start Fresh (Simplest for New Repos)

```powershell
# 1. Create a fresh repository on GitHub (or delete and recreate)

# 2. Remove old remote
git remote remove origin

# 3. Delete .git folder and start fresh
Remove-Item -Recurse -Force .git

# 4. Initialize new repo
git init
git add .
git commit -m "Initial commit - Clean version without secrets"

# 5. Add new remote
git remote add origin https://github.com/Saruf-Ratul/Services-API.git

# 6. Force push (overwrites remote)
git push -u origin main --force
```

### Method B: Use BFG Repo-Cleaner (Best for Existing Repos)

```powershell
# 1. Download BFG: https://rtyley.github.io/bfg-repo-cleaner/
#    Save as: bfg.jar (in your project folder)

# 2. Create replacement file
@"
ACXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX==>YOUR_TWILIO_ACCOUNT_SID
SKXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX==>YOUR_TWILIO_API_KEY
XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX==>YOUR_TWILIO_AUTH_TOKEN
"@ | Out-File -FilePath secrets.txt -Encoding UTF8

# 3. Clone a mirror (required by BFG)
cd ..
git clone --mirror https://github.com/Saruf-Ratul/Services-API.git Services-API.git
cd Services-API.git

# 4. Run BFG
java -jar ..\Services\bfg.jar --replace-text ..\Services\secrets.txt

# 5. Clean up
git reflog expire --expire=now --all
git gc --prune=now --aggressive

# 6. Force push
git push origin --force --all
cd ..
Remove-Item -Recurse -Force Services-API.git
cd Services
```

### Method C: Use git-filter-repo (Modern Alternative)

```powershell
# 1. Install git-filter-repo (requires Python)
pip install git-filter-repo

# 2. Create replacement file (same as Method B)

# 3. Run filter-repo
git filter-repo --replace-text secrets.txt

# 4. Force push
git push origin --force --all
```

---

## ✅ After Cleaning History

1. **Verify secrets are gone:**
   ```powershell
   git log --all --source -p | Select-String "AC[a-f0-9]\{32\}"
   ```
   Should return nothing.

2. **Push to GitHub:**
   ```powershell
   git push origin main --force
   ```

3. **Rotate compromised credentials:**
   - Generate new Twilio Account SID
   - Generate new Twilio Auth Token  
   - Generate new Twilio API Key
   - Update your actual config files with new values

---

## 📝 Recommendation

Since your repository appears to be new (few commits), I recommend **Method A (Start Fresh)** - it's the simplest and fastest solution.

If you want to preserve commit history, use **Method B (BFG)** or **Method C (git-filter-repo)**.

---

**Remember:** After cleaning, all secrets will be removed from Git history, but you should still rotate the actual credentials since they were exposed!

