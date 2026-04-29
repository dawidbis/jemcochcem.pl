# TaskComplete Hook
# Uruchamia się po zakończeniu zadania
# Cel: Uruchom testy + code review + sprawdź jakość kodu

try {
    $rawInput = [Console]::In.ReadToEnd()
    $taskData = $null
    if ($rawInput) {
        $taskData = $rawInput | ConvertFrom-Json
    }
} catch {
    Write-Error "[TaskComplete] Invalid JSON input: $($_.Exception.Message)"
}

$report = @()
$hasErrors = $false
$hasWarnings = $false

$report += "[CHECK] TaskComplete: Weryfikacja zakonczoneego zadania..."
$report += ""

# ============================================
# 1. BACKEND: Kompilacja + Testy
# ============================================
if (Test-Path "Backend") {
    $report += "[BUILD] Backend (C#):"
    
    # Kompilacja
    $report += "  → Kompilacja..."
    Push-Location "Backend"
    try {
        $buildOutput = dotnet build --no-restore 2>&1 | Out-String
        if ($LASTEXITCODE -ne 0) {
            $report += "     BLAD KOMPILACJI:"
            $report += $buildOutput
            $hasErrors = $true
        } else {
            $report += "     Kompilacja OK"
        }
    } catch {
        $report += "      Nie mozna uruchomic dotnet build"
        $hasWarnings = $true
    }
    Pop-Location
    
    # Testy
    if (Test-Path "Backend.tests") {
        $report += "  → Uruchamiam testy..."
        Push-Location "Backend.tests"
        try {
            $testOutput = dotnet test --no-build 2>&1 | Out-String
            if ($LASTEXITCODE -ne 0) {
                $report += "     TESTY NIE PRZESZLY:"
                $report += $testOutput
                $hasErrors = $true
            } else {
                $report += "     Wszystkie testy przeszly"
            }
        } catch {
            $report += "      Nie mozna uruchomic testow"
            $hasWarnings = $true
        }
        Pop-Location
    }
}

# ============================================
# 2. FRONTEND: Type-check + Lint + Testy
# ============================================
if (Test-Path "frontend") {
    $report += ""
    $report += "  Frontend (TypeScript):"
    
    Push-Location "frontend"
    
    # Type check
    $report += "  → Type check..."
    try {
        $typeOutput = npm run type-check 2>&1 | Out-String
        if ($LASTEXITCODE -ne 0) {
            $report += "     BLEDY TYPOW:"
            $report += $typeOutput
            $hasErrors = $true
        } else {
            $report += "     Typy OK"
        }
    } catch {
        $report += "      Brak skryptu type-check"
        $hasWarnings = $true
    }
    
    # Lint
    $report += "  → ESLint..."
    try {
        $lintOutput = npm run lint 2>&1 | Out-String
        if ($LASTEXITCODE -ne 0) {
            $report += "      Ostrzezenia ESLint"
            $hasWarnings = $true
        } else {
            $report += "     Linter OK"
        }
    } catch {
        $report += "      Brak skryptu lint"
    }
    
    # Testy
    $report += "  → Testy..."
    try {
        $testOutput = npm run test 2>&1 | Out-String
        if ($LASTEXITCODE -ne 0) {
            $report += "     TESTY NIE PRZESZLY:"
            $hasErrors = $true
        } else {
            $report += "     Testy OK"
        }
    } catch {
        $report += "      Brak skryptu test"
    }
    
    Pop-Location
}

# ============================================
# 3. CODE REVIEW - Szukaj typowych problemow
# ============================================
$report += ""
$report += "[REVIEW] Code Review:"

# Szukaj console.log w plikach TS/TSX
$consoleLogs = Get-ChildItem -Path "frontend/src" -Recurse -Include "*.ts","*.tsx" -ErrorAction SilentlyContinue | 
    Select-String -Pattern "console\.log" -ErrorAction SilentlyContinue

if ($consoleLogs) {
    $report += "   Znaleziono console.log() w:"
    $consoleLogs | ForEach-Object { $report += "     - $($_.Path):$($_.LineNumber)" }
    $hasErrors = $true
}

# Szukaj 'any' type
$anyTypes = Get-ChildItem -Path "frontend/src" -Recurse -Include "*.ts","*.tsx" -ErrorAction SilentlyContinue | 
    Select-String -Pattern ":\s*any\b" -ErrorAction SilentlyContinue

if ($anyTypes) {
    $report += "    Uzyto 'any' type w:"
    $anyTypes | Select-Object -First 5 | ForEach-Object { $report += "     - $($_.Path):$($_.LineNumber)" }
    $hasWarnings = $true
}

# Szukaj catch bez logowania w C#
$badCatches = Get-ChildItem -Path "Backend" -Recurse -Include "*.cs" -ErrorAction SilentlyContinue | 
    Select-String -Pattern "catch\s*\(\s*Exception\s*\)\s*\{" -ErrorAction SilentlyContinue

if ($badCatches) {
    $report += "    Catch bez logowania w:"
    $badCatches | Select-Object -First 5 | ForEach-Object { $report += "     - $($_.Path):$($_.LineNumber)" }
    $hasWarnings = $true
}

# ============================================
# 4. PODSUMOWANIE
# ============================================
$report += ""
$report += "======================================="
if ($hasErrors) {
    $report += "[ERROR] ZNALEZIONO BLEDY KRYTYCZNE - NAPRAW PRZED COMMITEM!"
} elseif ($hasWarnings) {
    $report += "[WARN] Znaleziono ostrzezenia - rozwaz poprawienie"
} else {
    $report += "[OK] Wszystko OK - kod gotowy do commita!"
}
$report += "======================================="

$reportText = $report -join "`n"

@{
    cancel = $false
    contextModification = $reportText
    errorMessage = if ($hasErrors) { "Znaleziono bledy w kodzie" } else { "" }
} | ConvertTo-Json -Compress
