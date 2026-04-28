# TaskStart Hook
# Uruchamia się na początku każdego zadania
# Cel: Załadowanie kontekstu projektu (README.md + plantUML)

try {
    $rawInput = [Console]::In.ReadToEnd()
    if ($rawInput) {
        $null = $rawInput | ConvertFrom-Json
    }
} catch {
    Write-Error "[TaskStart] Invalid JSON input: $($_.Exception.Message)"
}

# ============================================
# Załaduj zawartość README.md i plantUML
# ============================================

$contextAddition = ""

# README.md
$readmePath = "README.md"
if (Test-Path $readmePath) {
    $readmeContent = Get-Content $readmePath -Raw -ErrorAction SilentlyContinue
    if ($readmeContent) {
        $contextAddition += "`n=== README.md (Projekt FitApp) ===`n"
        $contextAddition += $readmeContent
        $contextAddition += "`n=== END README ===`n"
    }
}

# plantUML-kod.txt
$umlPath = "UML/plantUML-kod.txt"
if (Test-Path $umlPath) {
    $umlContent = Get-Content $umlPath -Raw -ErrorAction SilentlyContinue
    if ($umlContent) {
        $contextAddition += "`n=== plantUML-kod.txt (Architektura) ===`n"
        $contextAddition += $umlContent
        $contextAddition += "`n=== END UML ===`n"
    }
}

# Dodaj instrukcje
$contextAddition += @"

=== INSTRUKCJE DLA CLINE ===
1. Przeczytałeś README.md i plantUML powyżej
2. Sprawdź obecną iterację (MVP/Analytics/AI) przed implementacją
3. Stosuj Clean Architecture i CQRS
4. Używaj poprawnych konwencji nazewnictwa
5. Dodawaj komentarze (CO, DLACZEGO, JAK)
6. Pisz testy do nowego kodu
=== END INSTRUKCJE ===
"@

@{
    cancel = $false
    contextModification = $contextAddition
    errorMessage = ""
} | ConvertTo-Json -Compress