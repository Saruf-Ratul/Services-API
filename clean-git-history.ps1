# Script to clean secrets from Git history using git filter-branch

Write-Host "=========================================" -ForegroundColor Cyan
Write-Host "Git History Cleaner for Secrets" -ForegroundColor Yellow
Write-Host "=========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "This script will remove secrets from ALL Git history." -ForegroundColor Yellow
Write-Host "WARNING: This rewrites Git history. Make sure you understand the implications!" -ForegroundColor Red
Write-Host ""

$confirm = Read-Host "Do you want to proceed? (yes/no)"
if ($confirm -ne "yes") {
    Write-Host "Aborted." -ForegroundColor Yellow
    exit 0
}

Write-Host ""
Write-Host "Step 1: Creating backup branch..." -ForegroundColor Yellow
git branch backup-before-cleanup
Write-Host "  ✓ Backup created" -ForegroundColor Green

Write-Host ""
Write-Host "Step 2: Removing secrets from all commits..." -ForegroundColor Yellow

# Remove secrets from all files in all commits
# IMPORTANT: Replace these with YOUR actual exposed secrets
$secrets = @{
    "AC[REPLACE_WITH_YOUR_EXPOSED_ACCOUNT_SID]" = "YOUR_TWILIO_ACCOUNT_SID"
    "SK[REPLACE_WITH_YOUR_EXPOSED_API_KEY]" = "YOUR_TWILIO_API_KEY"
    "[REPLACE_WITH_YOUR_EXPOSED_AUTH_TOKEN]" = "YOUR_TWILIO_AUTH_TOKEN"
}

# Use git filter-repo if available, otherwise use filter-branch
$useFilterRepo = Get-Command git-filter-repo -ErrorAction SilentlyContinue

if ($useFilterRepo) {
    Write-Host "  Using git-filter-repo (recommended)..." -ForegroundColor Green
    
    # Create replacement file
    $replacements = @()
    foreach ($key in $secrets.Keys) {
        $replacements += "$key==>$($secrets[$key])"
    }
    $replacements | Out-File -FilePath "secrets-replacements.txt" -Encoding UTF8
    
    Write-Host "  Running git-filter-repo..." -ForegroundColor Gray
    git filter-repo --replace-text secrets-replacements.txt --force
    
    Remove-Item "secrets-replacements.txt" -ErrorAction SilentlyContinue
} else {
    Write-Host "  Using git filter-branch (slower)..." -ForegroundColor Yellow
    Write-Host "  Note: Install git-filter-repo for better performance: pip install git-filter-repo" -ForegroundColor Gray
    
    $filterScript = @"
# Replace secrets in all files
# NOTE: Update these with your actual exposed secrets before running
sed -i 's/AC[YOUR_EXPOSED_SID]/YOUR_TWILIO_ACCOUNT_SID/g'
sed -i 's/SK[YOUR_EXPOSED_KEY]/YOUR_TWILIO_API_KEY/g'
sed -i 's/[YOUR_EXPOSED_TOKEN]/YOUR_TWILIO_AUTH_TOKEN/g'
"@
    
    $filterScript | Out-File -FilePath ".git-filterscript.sh" -Encoding UTF8
    
    Write-Host "  Running git filter-branch (this may take a while)..." -ForegroundColor Gray
    Write-Host "  ERROR: You must edit this script first to add your actual exposed secrets!" -ForegroundColor Red
    Write-Host "  Edit the sed commands above with your real secret values." -ForegroundColor Yellow
    exit 1
    
    Remove-Item ".git-filterscript.sh" -ErrorAction SilentlyContinue
}

Write-Host ""
Write-Host "Step 3: Cleaning up..." -ForegroundColor Yellow
git reflog expire --expire=now --all
git gc --prune=now --aggressive

Write-Host ""
Write-Host "=========================================" -ForegroundColor Cyan
Write-Host "History cleaned!" -ForegroundColor Green
Write-Host "=========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Next steps:" -ForegroundColor Yellow
Write-Host "  1. Verify: git log --all --source --grep='secret'" -ForegroundColor White
Write-Host "  2. Force push: git push origin --force --all" -ForegroundColor White
Write-Host "  3. If you have tags: git push origin --force --tags" -ForegroundColor White
Write-Host ""
Write-Host "WARNING: Force push rewrites remote history!" -ForegroundColor Red
Write-Host "Make sure all collaborators know about this change." -ForegroundColor Yellow
Write-Host ""

