# Start Fresh - Remove all Git history and create one clean commit

Write-Host "=========================================" -ForegroundColor Cyan
Write-Host "Start Fresh - Clean Git History" -ForegroundColor Yellow
Write-Host "=========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "This will:" -ForegroundColor Yellow
Write-Host "  1. Backup current .git folder" -ForegroundColor White
Write-Host "  2. Remove all Git history" -ForegroundColor White
Write-Host "  3. Create ONE clean commit with current files" -ForegroundColor White
Write-Host "  4. Prepare for force push to GitHub" -ForegroundColor White
Write-Host ""

$confirm = Read-Host "Continue? (yes/no)"
if ($confirm -ne "yes") {
    Write-Host "Aborted." -ForegroundColor Yellow
    exit 0
}

Write-Host ""
Write-Host "Step 1: Creating backup..." -ForegroundColor Yellow
$backupDir = ".git-backup-$(Get-Date -Format 'yyyyMMdd-HHmmss')"
Copy-Item -Path ".git" -Destination $backupDir -Recurse -Force
Write-Host "  ✓ Backup created: $backupDir" -ForegroundColor Green

Write-Host ""
Write-Host "Step 2: Removing old Git history..." -ForegroundColor Yellow
Remove-Item -Recurse -Force .git
Write-Host "  ✓ Old history removed" -ForegroundColor Green

Write-Host ""
Write-Host "Step 3: Initializing new repository..." -ForegroundColor Yellow
git init
Write-Host "  ✓ New repository initialized" -ForegroundColor Green

Write-Host ""
Write-Host "Step 4: Staging all files..." -ForegroundColor Yellow
git add .
Write-Host "  ✓ Files staged" -ForegroundColor Green

Write-Host ""
Write-Host "Step 5: Creating clean commit..." -ForegroundColor Yellow
git commit -m "Initial commit - Clean version without secrets"
Write-Host "  ✓ Clean commit created" -ForegroundColor Green

Write-Host ""
Write-Host "Step 6: Setting up remote..." -ForegroundColor Yellow
git remote remove origin 2>$null
git remote add origin https://github.com/Saruf-Ratul/Services-API.git
Write-Host "  ✓ Remote configured" -ForegroundColor Green

Write-Host ""
Write-Host "Step 7: Setting branch name..." -ForegroundColor Yellow
git branch -M main
Write-Host "  ✓ Branch set to 'main'" -ForegroundColor Green

Write-Host ""
Write-Host "=========================================" -ForegroundColor Cyan
Write-Host "✓ READY TO PUSH!" -ForegroundColor Green
Write-Host "=========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Next command to run:" -ForegroundColor Yellow
Write-Host "  git push -u origin main --force" -ForegroundColor Cyan
Write-Host ""
Write-Host "⚠️  WARNING: --force will overwrite remote repository!" -ForegroundColor Red
Write-Host "This is safe since you're starting fresh." -ForegroundColor Gray
Write-Host ""
Write-Host "Backup saved in: $backupDir" -ForegroundColor Gray
Write-Host ""

