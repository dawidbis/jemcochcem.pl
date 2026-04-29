# UserPromptSubmit Hook
# Uruchamia się po wysłaniu wiadomości przez użytkownika
# Cel: Dodaj przypomnienia o zasadach projektu

try {
    $rawInput = [Console]::In.ReadToEnd()
    $promptData = $null
    if ($rawInput) {
        $promptData = $rawInput | ConvertFrom-Json
    }
} catch {
    Write-Error "[UserPromptSubmit] Invalid JSON input: $($_.Exception.Message)"
    @{ cancel = $false; contextModification = ""; errorMessage = "" } | ConvertTo-Json -Compress
    exit 0
}

# Pobierz wiadomość użytkownika
$userMessage = ""
if ($promptData.prompt) {
    $userMessage = $promptData.prompt.ToLower()
}

$reminders = @()

# ============================================
# Dodaj kontekstowe przypomnienia
# ============================================

# Jesli user pyta o nowy feature/funkcje
if ($userMessage -match "(dodaj|stworz|nowa? funkcj|feature|implementuj)") {
    $reminders += " PRZYPOMNIENIE - Tworzenie nowej funkcji:"
    $reminders += "  [OK] Sprawdz obecna iteracje w README.md (MVP/Analytics/AI)"
    $reminders += "  [OK] Zastosuj Clean Architecture (Domain -> Application -> Infrastructure -> API)"
    $reminders += "  [OK] Backend: Command + Handler + Validator (CQRS z MediatR)"
    $reminders += "  [OK] Frontend: Component + Hook + API client w features/"
    $reminders += "  [OK] Dodaj testy jednostkowe"
    $reminders += "  [OK] Komentarze: CO, DLACZEGO, JAK"
}

# Jesli user pyta o naprawe bledu
if ($userMessage -match "(napraw|fix|blad|bug|problem|nie dziala)") {
    $reminders += " PRZYPOMNIENIE - Naprawa bledu (TDD):"
    $reminders += "  1. Najpierw napisz test reprodukujacy bug (RED)"
    $reminders += "  2. Napraw kod (GREEN)"
    $reminders += "  3. Refactor jesli potrzeba (REFACTOR)"
    $reminders += "  4. Dodaj komentarz wyjasniajacy fix"
}

# Jesli user pyta o refaktoryzacje
if ($userMessage -match "(refaktor|refactor|popraw|optymalizuj)") {
    $reminders += " PRZYPOMNIENIE - Refaktoryzacja:"
    $reminders += "  [OK] Najpierw upewnij sie ze sa testy"
    $reminders += "  [OK] Refaktoryzuj malymi krokami"
    $reminders += "  [OK] Po kazdym kroku: testy musza przejsc"
    $reminders += "  [OK] Nie zmieniaj funkcjonalnosci"
}

# Jesli user pyta o testy
if ($userMessage -match "(test|testy|testowanie)") {
    $reminders += " PRZYPOMNIENIE - Testy:"
    $reminders += "  [OK] Backend: xUnit + Moq + FluentAssertions"
    $reminders += "  [OK] Frontend: Vitest + Testing Library"
    $reminders += "  [OK] Pattern: Arrange / Act / Assert"
    $reminders += "  [OK] Testuj edge cases (null, empty, errors)"
}

# Jesli user pyta o commit
if ($userMessage -match "(commit|commitowac|wypchnij|push)") {
    $reminders += " PRZYPOMNIENIE - Przed commitem:"
    $reminders += "  [OK] dotnet build (Backend)"
    $reminders += "  [OK] npm run type-check (Frontend)"
    $reminders += "  [OK] Brak console.log()"
    $reminders += "  [OK] Format: type(scope): message"
    $reminders += "  [OK] Przyklad: feat(diet): dodano logowanie posilkow"
}

# Jesli user pyta o baze danych
if ($userMessage -match "(baza|database|migracja|migration|ef core|entity)") {
    $reminders += "  PRZYPOMNIENIE - Praca z baza:"
    $reminders += "  [OK] Zmiana entity -> dodaj migracje:"
    $reminders += "    dotnet ef migrations add NazwaMigracji"
    $reminders += "  [OK] Aktualizuj baze:"
    $reminders += "    dotnet ef database update"
    $reminders += "  [OK] Repository implementuj IRepository z Domain"
}

# Jesli user pyta o AI/Claude
if ($userMessage -match "(ai|claude|chatbot|gpt|openai)") {
    $reminders += " PRZYPOMNIENIE - Integracja AI:"
    $reminders += "  [OK] ClaudeAiService w FitApp.Infrastructure"
    $reminders += "  [OK] API key z appsettings (NIGDY hardcoded!)"
    $reminders += "  [OK] Obsluz timeouts (30s default)"
    $reminders += "  [OK] Cache odpowiedzi gdy mozliwe"
}

# ============================================
# Zwróć kontekst
# ============================================
$contextAddition = ""
if ($reminders.Count -gt 0) {
    $contextAddition = "`n=== AUTO-PRZYPOMNIENIA ===`n"
    $contextAddition += ($reminders -join "`n")
    $contextAddition += "`n=== END ===`n"
}

@{
    cancel = $false
    contextModification = $contextAddition
    errorMessage = ""
} | ConvertTo-Json -Compress