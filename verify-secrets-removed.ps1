# Verify Secrets Have Been Removed

Write-Host "=========================================" -ForegroundColor Cyan
Write-Host "Secret Verification Script" -ForegroundColor Yellow
Write-Host "=========================================" -ForegroundColor Cyan
Write-Host ""

$secretsFound = $false

# Check for Twilio Account SID pattern
Write-Host "Checking for Twilio Account SID..." -ForegroundColor Yellow
$accountSid = git grep -i "AC[a-z0-9]\{32\}" 2>&1
if ($accountSid) {
    Write-Host "  WARNING: Twilio Account SID found!" -ForegroundColor Red
    Write-Host "  $accountSid" -ForegroundColor Red
    $secretsFound = $true
} else {
    Write-Host "  OK: No Twilio Account SID found" -ForegroundColor Green
}

# Check for Twilio API Key pattern
Write-Host "Checking for Twilio API Key..." -ForegroundColor Yellow
$apiKey = git grep -i "SK[a-z0-9]\{32\}" 2>&1
if ($apiKey) {
    Write-Host "  WARNING: Twilio API Key found!" -ForegroundColor Red
    Write-Host "  $apiKey" -ForegroundColor Red
    $secretsFound = $true
} else {
    Write-Host "  OK: No Twilio API Key found" -ForegroundColor Green
}

# Check for secret patterns (checking patterns only, not storing actual values)
Write-Host "Checking for secret patterns..." -ForegroundColor Yellow
$secretPatterns = @(
    "AC[a-f0-9]{32}",
    "SK[a-f0-9]{32}"
)

foreach ($pattern in $secretPatterns) {
    $found = git grep -E "$pattern" 2>&1 | Where-Object { $_ -notmatch "YOUR_TWILIO" -and $_ -notmatch "placeholder" -and $_ -notmatch "REDACTED" }
    if ($found) {
        Write-Host "  WARNING: Secret pattern match found!" -ForegroundColor Red
        Write-Host "  $found" -ForegroundColor Red
        $secretsFound = $true
    }
}

Write-Host ""
Write-Host "=========================================" -ForegroundColor Cyan

if ($secretsFound) {
    Write-Host "SECRETS STILL FOUND - Please remove them!" -ForegroundColor Red
    Write-Host "=========================================" -ForegroundColor Cyan
    exit 1
} else {
    Write-Host "NO SECRETS FOUND - Safe to commit!" -ForegroundColor Green
    Write-Host "=========================================" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "Next steps:" -ForegroundColor Yellow
    Write-Host "  1. Review changed files: git status" -ForegroundColor White
    Write-Host "  2. Stage changes: git add ." -ForegroundColor White
    Write-Host "  3. Commit: git commit -m 'Remove secrets and replace with placeholders'" -ForegroundColor White
    Write-Host "  4. Push: git push origin main" -ForegroundColor White
    Write-Host ""
    exit 0
}

