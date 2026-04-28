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

# Jeśli user pyta o nowy feature/funkcję
if ($userMessage -match "(dodaj|stwórz|nowa? funkcj|feature|implementuj)") {
    $reminders += "📋 PRZYPOMNIENIE - Tworzenie nowej funkcji:"
    $reminders += "  ✓ Sprawdź obecną iterację w README.md (MVP/Analytics/AI)"
    $reminders += "  ✓ Zastosuj Clean Architecture (Domain → Application → Infrastructure → API)"
    $reminders += "  ✓ Backend: Command + Handler + Validator (CQRS z MediatR)"
    $reminders += "  ✓ Frontend: Component + Hook + API client w features/"
    $reminders += "  ✓ Dodaj testy jednostkowe"
    $reminders += "  ✓ Komentarze: CO, DLACZEGO, JAK"
}

# Jeśli user pyta o naprawę błędu
if ($userMessage -match "(napraw|fix|błąd|bug|problem|nie działa)") {
    $reminders += "🐛 PRZYPOMNIENIE - Naprawa błędu (TDD):"
    $reminders += "  1. Najpierw napisz test reprodukujący bug (RED)"
    $reminders += "  2. Napraw kod (GREEN)"
    $reminders += "  3. Refactor jeśli potrzeba (REFACTOR)"
    $reminders += "  4. Dodaj komentarz wyjaśniający fix"
}

# Jeśli user pyta o refaktoryzację
if ($userMessage -match "(refaktor|refactor|popraw|optymalizuj)") {
    $reminders += "🔧 PRZYPOMNIENIE - Refaktoryzacja:"
    $reminders += "  ✓ Najpierw upewnij się że są testy"
    $reminders += "  ✓ Refaktoryzuj małymi krokami"
    $reminders += "  ✓ Po każdym kroku: testy muszą przejść"
    $reminders += "  ✓ Nie zmieniaj funkcjonalności"
}

# Jeśli user pyta o testy
if ($userMessage -match "(test|testy|testowanie)") {
    $reminders += "🧪 PRZYPOMNIENIE - Testy:"
    $reminders += "  ✓ Backend: xUnit + Moq + FluentAssertions"
    $reminders += "  ✓ Frontend: Vitest + Testing Library"
    $reminders += "  ✓ Pattern: Arrange / Act / Assert"
    $reminders += "  ✓ Testuj edge cases (null, empty, errors)"
}

# Jeśli user pyta o commit
if ($userMessage -match "(commit|commitować|wypchnij|push)") {
    $reminders += "📝 PRZYPOMNIENIE - Przed commitem:"
    $reminders += "  ✓ dotnet build (Backend)"
    $reminders += "  ✓ npm run type-check (Frontend)"
    $reminders += "  ✓ Brak console.log()"
    $reminders += "  ✓ Format: type(scope): message"
    $reminders += "  ✓ Przykład: feat(diet): dodano logowanie posiłków"
}

# Jeśli user pyta o bazę danych
if ($userMessage -match "(baza|database|migracja|migration|ef core|entity)") {
    $reminders += "🗄️  PRZYPOMNIENIE - Praca z bazą:"
    $reminders += "  ✓ Zmiana entity → dodaj migrację:"
    $reminders += "    dotnet ef migrations add NazwaMigracji"
    $reminders += "  ✓ Aktualizuj bazę:"
    $reminders += "    dotnet ef database update"
    $reminders += "  ✓ Repository implementuj IRepository z Domain"
}

# Jeśli user pyta o AI/Claude
if ($userMessage -match "(ai|claude|chatbot|gpt|openai)") {
    $reminders += "🤖 PRZYPOMNIENIE - Integracja AI:"
    $reminders += "  ✓ ClaudeAiService w FitApp.Infrastructure"
    $reminders += "  ✓ API key z appsettings (NIGDY hardcoded!)"
    $reminders += "  ✓ Obsłuż timeouts (30s default)"
    $reminders += "  ✓ Cache odpowiedzi gdy możliwe"
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