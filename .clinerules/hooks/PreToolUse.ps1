# PreToolUse Hook
# Uruchamia się przed każdym użyciem narzędzia przez Cline
# Cel: Blokowanie groźnych operacji (rm -rf, force push, etc.)

try {
    $rawInput = [Console]::In.ReadToEnd()
    $toolData = $null
    if ($rawInput) {
        $toolData = $rawInput | ConvertFrom-Json
    }
} catch {
    Write-Error "[PreToolUse] Invalid JSON input: $($_.Exception.Message)"
    @{ cancel = $false; contextModification = ""; errorMessage = "" } | ConvertTo-Json -Compress
    exit 0
}

$shouldCancel = $false
$cancelReason = ""
$warnings = @()

# Pobierz informacje o narzędziu
$toolName = $toolData.toolName
$toolInput = $toolData.toolInput | ConvertTo-Json -Compress

# ============================================
# BLOKUJ: Niebezpieczne komendy bash
# ============================================
if ($toolName -eq "execute_command" -or $toolName -eq "bash") {
    $command = $toolData.toolInput.command
    
    # Blokuj rm -rf na ważnych folderach
    if ($command -match "rm\s+-rf\s+(/|~|\.\.|Backend|frontend|UML)") {
        $shouldCancel = $true
        $cancelReason = " ZABLOKOWANO: Niebezpieczne usuwanie ($command)"
    }
    
    # Blokuj force push do main/master
    if ($command -match "git\s+push\s+.*--force.*\b(main|master)\b") {
        $shouldCancel = $true
        $cancelReason = " ZABLOKOWANO: Force push do main/master jest niebezpieczny!"
    }
    
    # Blokuj git reset --hard bez backup
    if ($command -match "git\s+reset\s+--hard") {
        $warnings += "  UWAGA: git reset --hard usunie wszystkie nieskommitowane zmiany!"
    }
    
    # Blokuj operacje na .env
    if ($command -match "(rm|cat|echo).*\.env") {
        $warnings += "  UWAGA: Operacja na pliku .env - może zawierać sekrety!"
    }
    
    # Blokuj instalację globalnych pakietów bez pytania
    if ($command -match "npm\s+install\s+-g") {
        $warnings += "  UWAGA: Globalna instalacja npm package - upewnij się że to bezpieczne!"
    }
}

# ============================================
# OSTRZEŻENIA: Operacje na ważnych plikach
# ============================================
if ($toolName -eq "write_to_file" -or $toolName -eq "replace_in_file") {
    $filePath = $toolData.toolInput.path
    
    # Ostrzeżenie dla plików konfiguracyjnych
    $criticalFiles = @(
        "appsettings.json",
        "appsettings.Production.json",
        ".env",
        ".env.production",
        "package.json",
        "FitApp.sln",
        "docker-compose.yml",
        ".gitignore"
    )
    
    foreach ($critical in $criticalFiles) {
        if ($filePath -like "*$critical*") {
            $warnings += "  Modyfikacja krytycznego pliku: $critical"
        }
    }
    
    # Blokuj modyfikacje w node_modules / bin / obj
    if ($filePath -match "(node_modules|bin\\Debug|obj\\Debug)") {
        $shouldCancel = $true
        $cancelReason = " ZABLOKOWANO: Nie modyfikuj plików w $filePath"
    }
}

# ============================================
# Zwróć wynik
# ============================================
$contextMod = ""
if ($warnings.Count -gt 0) {
    $contextMod = ($warnings -join "`n")
}

@{
    cancel = $shouldCancel
    contextModification = $contextMod
    errorMessage = $cancelReason
} | ConvertTo-Json -Compress