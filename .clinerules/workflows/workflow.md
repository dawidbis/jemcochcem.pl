# 🔄 Workflows — Sekwencje Zadań

Workflows to zautomatyzowane sekwencje kroków dla powtarzalnych procesów.

**Trigger:** Wpisz `/workflow-name` w chacie Cline

---

## 🆕 /feature - Nowa funkcja

**Trigger:** `/feature`

**Kroki:**
1. Czyta `README.md` → sprawdza iterację (MVP/Analytics/AI)
2.  Czyta `UML/plantUML-kod.txt` → planuje architekturę
3.  Szuka podobnego kodu w projekcie
4. Pyta użytkownika o szczegóły:
   - Nazwa feature?
   - Warstwa (Backend/Frontend/Both)?
   - Dodatkowe wymagania?
5.  Tworzy strukturę:
   - **Backend:** Command + Handler + Validator + Controller
   - **Frontend:** Component + Hook + API client + Types
6.  Generuje testy:
   - Backend: xUnit tests
   - Frontend: Vitest tests
7.  Dodaje komentarze do każdego pliku
8.  Uruchamia pre-commit checks
9.  Tworzy checklist do code review

**Przykład użycia:**
```
User: /feature

Cline: Jaka nazwa feature?
User: User profile editing

Cline: Backend, Frontend, czy Both?
User: Both

Cline: [tworzy Command, Handler, Component, Hook, testy...]
Cline:  Feature gotowy! Checklist do review:
       - [ ] Testy przechodzą
       - [ ] Komentarze dodane
       - [ ] Clean Architecture zachowana
```

---

##  /bugfix - Naprawa błędu

**Trigger:** `/bugfix`

**Kroki:**
1.  Pyta o opis buga
2.  Szuka podobnych bugów w issues/commits
3.  Lokalizuje plik z bugiem
4.  Tworzy test reprodukujący bug (RED)
5.  Naprawia kod
6.  Sprawdza czy test przechodzi (GREEN)
7.  Refactor jeśli potrzebny
8.  Dodaje komentarz wyjaśniający fix
9.  Uruchamia wszystkie testy
10.  Generuje commit message

**Przykład:**
```
User: /bugfix

Cline: Opisz bug:
User: Dziennik nie pokazuje posiłków po dodaniu

Cline: [Szuka w frontend/src/features/diet/...]
Cline: Znalazłem problem w useDiary.ts - brak invalidateQueries
Cline: [Tworzy test reprodukujący bug]
Cline: [Naprawia kod]
Cline:  Test przechodzi!
Cline: Sugerowany commit: "fix(diet): dodano invalidateQueries po dodaniu posiłku"
```

---

##  /test-gen - Generowanie testów

**Trigger:** `/test-gen`

**Kroki:**
1. Skanuje pliki bez testów
2. Dla każdego pliku:
   - Backend: Generuje xUnit test z Arrange/Act/Assert
   - Frontend: Generuje Vitest test z render/expect
3. Dodaje TODO: dla przypadków brzegowych
4. Uruchamia testy
5. Pokazuje coverage

**Przykład:**
```
User: /test-gen

Cline: Znaleziono 3 pliki bez testów:
       - AddMealItemCommandHandler.cs
       - MealLogItem.tsx
       - useDiary.ts
       
Cline: [Generuje testy...]
Cline:  Wygenerowano 3 pliki testowe
Cline: Coverage: 67% → 85%
Cline:  Sprawdź TODO w testach i uzupełnij edge cases
```

---

##  /refactor - Refaktoryzacja

**Trigger:** `/refactor [ścieżka]`

**Kroki:**
1. Analizuje wskazany kod
2. Identyfikuje code smells:
   - Długie metody (>50 linii)
   - Zagnieżdżone if'y (>3 poziomy)
   - DRY violations
   - Magic numbers
3. Proponuje refactoring strategy
4. Wykonuje zmiany krok po kroku
5. Uruchamia testy po każdym kroku (musi być GREEN!)
6. Aktualizuje komentarze

**Przykład:**
```
User: /refactor Backend/FitApp.Application/Features/Diet/Commands/AddMealItemCommandHandler.cs

Cline: Analizuję plik...
Cline: Znalazłem 3 problemy:
       1. Method Handle() ma 78 linii - rozdzielić
       2. Duplikacja logiki walidacji (linie 23-30 i 45-52)
       3. Magic number 10000 w linii 67
       
Cline: Plan refaktoryzacji:
       1. Wyciągnij ValidateInput() do osobnej metody
       2. Wyciągnij CalculateMacros() do osobnej metody
       3. Stwórz const MAX_PORTION = 10000
       
Cline: Wykonać? [tak/nie]
User: tak

Cline: [Refaktoryzuje krok po kroku]
Cline:  Wszystkie testy przechodzą
```

---

##  /release - Przygotowanie release

**Trigger:** `/release`

**Kroki:**
1.  Wszystkie testy przechodzą
2.  Brak `console.log()` w kodzie
3.  Wszystkie TODO rozwiązane lub udokumentowane
4.  Generuje CHANGELOG.md
5.  Sugeruje numer wersji (semantic versioning)
6.  Tworzy release notes
7.  Przygotowuje build production

---

##  /review - Code Review przed PR

**Trigger:** `/review`

**Kroki:**
1. Sprawdza wszystkie zmienione pliki (git diff)
2. Uruchamia `@code-reviewer` skill
3. Uruchamia `@security-scanner` skill
4. Sprawdza Clean Architecture boundaries
5. Generuje raport PR-ready

**Output:**
```
 PRE-PR REVIEW REPORT

Changed files: 5
Lines added: +234
Lines removed: -45

 Code Quality: 8/10
 Test Coverage: 87%
 Security: No issues
  Performance: 1 warning
 Architecture: OK

 PR Checklist:
- [x] Tests passing
- [x] Comments added
- [x] No console.log
- [x] Naming conventions OK
- [ ] Manual testing needed

 Ready for PR? Tak (po manualnym teście)
```

---

##  /e2e - Testy E2E

**Trigger:** `/e2e`

**Kroki:**
1. Uruchamia dev server (npm run dev)
2. Otwiera browser w trybie headless
3. Wykonuje user flow (np. login → add meal → check diary)
4. Zbiera screenshoty
5. Zbiera console logs
6. Sprawdza network requests
7. Generuje raport

**Wymagania:**
- Działający frontend (`npm run dev`)
- Działający backend (`dotnet run`)

---

##  Tworzenie własnego workflow

**Plik:** `.cline/workflows/custom-name.md`

```markdown
# /custom-name

**Trigger:** /custom-name

## Kroki
1. Krok pierwszy
2. Krok drugi
3. Krok trzeci

## Przykład
[użycie]
```

---

##  Lista wszystkich workflow

| Trigger | Opis |
|---------|------|
| `/feature` | Nowa funkcja od zera |
| `/bugfix` | Naprawa błędu z testem |
| `/test-gen` | Generowanie brakujących testów |
| `/refactor` | Refaktoryzacja kodu |
| `/release` | Przygotowanie release |
| `/review` | Code review przed PR |
| `/e2e` | Testy E2E |

---

**Ostatnia aktualizacja:** 2026-04-28