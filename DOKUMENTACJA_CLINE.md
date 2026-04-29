#  Dokumentacja Cline - Projekt FitApp

##  Czym jest Cline?

Cline to AI asystent programisty zintegrowany z VS Code. Pomaga w:
- Pisaniu kodu zgodnie z Clean Architecture
- Code review i refaktoryzacji
- Tworzeniu testów
- Automatyzacji zadań programistycznych

---

##  Struktura Projektu

### `.clinerules/` - Zasady i Konfiguracja

```
.clinerules/
├── rules.md                    # Główne zasady kodowania
├── hooks/                      # Hooki PowerShell
│   ├── TaskStart.ps1          # Uruchamia się przed zadaniem
│   ├── TaskComplete.ps1       # Uruchamia się po zadaniu
│   ├── PreToolUse.ps1         # Przed użyciem narzędzia
│   └── UserPromptSubmit.ps1   # Po wysłaniu promptu
└── workflows/
    └── workflow.md            # Workflow zarządzania zadaniami
```

### `.agents/skills/` - Umiejętności Cline

```
.agents/skills/
├── feature/        # Implementacja nowych funkcjonalności
├── refactor/       # Refaktoryzacja kodu
├── review/         # Code review
├── test/           # Testy jednostkowe/integracyjne
├── test_online/    # Testowanie API online
└── skills/         # Meta-skill (zarządzanie skillami)
```

---

##  Jak Używać Cline?

### 1. Podstawowe Komendy

#### Implementacja Funkcjonalności
```
"Zaimplementuj endpoint do dodawania posiłków"
"Dodaj walidację dla AddMealItemCommand"
"Stwórz UserRepository w Infrastructure"
```
→ Użyje skill `feature`

#### Code Review
```
"Przeprowadź code review DiaryController"
"Sprawdź czy kod przestrzega Clean Architecture"
"Zreviewuj walidatory w Features/Diet"
```
→ Użyje skill `review`

#### Refaktoryzacja
```
"Zrefaktoruj metodę CalculateTdee"
"Wydziel logikę do osobnej klasy"
"Popraw naming w UserService"
```
→ Użyje skill `refactor`

#### Testy
```
"Napisz testy dla AddMealItemCommandHandler"
"Dodaj testy integracyjne dla DiaryController"
"Przetestuj walidację AddMealItemValidator"
```
→ Użyje skill `test`

---

##  Zasady Komunikacji

###  Styl Neandertalczyka

Cline komunikuje się **krótko i konkretnie**:

```
DOBRZE:
"Ja robić endpoint. Dodać AddMealItemCommand. Testować dotnet build."

 ŹLE:
"Przystępuję do implementacji endpointu, który będzie obsługiwał 
dodawanie posiłków. W ramach tego zadania utworzę klasę..."
```

**Zasady:**
- Krótkie zdania (max 10 słów)
- Czasowniki w bezokoliczniku: "robić", "dodać", "testować"
- Konkretne akcje: co, gdzie, jak
- Bez ozdobników: "przystępuję", "w ramach", "zgodnie z"

---

##  Clean Architecture - Zasady

### Struktura Warstw

```
Backend/
├── FitApp.Domain/           #  CORE - zero dependencies!
│   ├── Entities/            # User.cs, MealLog.cs
│   ├── ValueObjects/        # MacroNutrients.cs
│   └── Interfaces/          # IUserRepository.cs
│
├── FitApp.Application/      # CQRS (MediatR)
│   └── Features/
│       └── Diet/
│           ├── Commands/    # AddMealItemCommand.cs + Handler
│           ├── Queries/     # GetDailyDiaryQuery.cs + Handler
│           └── Validators/  # AddMealItemValidator.cs
│
├── FitApp.Infrastructure/   # Implementacje
│   ├── Data/Repositories/   # UserRepository.cs
│   └── ExternalServices/    # OpenFoodFactsClient.cs
│
└── FitApp.API/             # Controllers (cienkie!)
    └── Controllers/         # DiaryController.cs
```

### Nazewnictwo Plików

#### Backend (C#)
```
AddMealItemCommand.cs              # PascalCase + Command suffix
AddMealItemCommandHandler.cs       # PascalCase + Handler suffix
AddMealItemValidator.cs            # PascalCase + Validator suffix
IUserRepository.cs                 # Interface: I + PascalCase
UserRepository.cs                  # Implementacja bez I
```

#### Frontend (TypeScript/React)
```
MealLogItem.tsx                    # Komponenty: PascalCase
useAuth.ts                         # Hooki: camelCase + 'use' prefix
diary-api.ts                       # API clients: kebab-case
DiaryTypes.ts                      # Typy: PascalCase + Types suffix
```

---

##  Stack Technologiczny

### Backend
- **C# ASP.NET Core 8** → REST API
- **EF Core 8** → ORM (SQL Server)
- **MediatR** → CQRS pattern
- **FluentValidation** → Walidacja Commands
- **Redis** → Cache (Open Food Facts API)
- **JWT** → Autoryzacja (Access + Refresh tokens)

### Frontend
- **React 18** → UI (Vite)
- **TypeScript** → Strict mode
- **TanStack Query** → Server state management
- **Tailwind CSS** → Styling (utility-first)
- **shadcn/ui** → Component library
- **Recharts** → Wykresy (waga, makroskładniki)

---

##  Checklist Przed/Po Kodzie

### Przed kodem:
- [ ] Przeczytaj `README.md` → Stack + Plan iteracji
- [ ] Sprawdź `UML/plantUML-kod.txt` → Architektura
- [ ] Wyszukaj podobny kod (`Ctrl+Shift+F`)
- [ ] Sprawdź naming conventions

### Po kodzie:
- [ ] Backend: `dotnet build` bez błędów
- [ ] Frontend: `npm run type-check` bez błędów
- [ ] Brak `console.log()` w kodzie
- [ ] **KOMENTARZE dodane** (CO, DLACZEGO, JAK)
- [ ] Testy napisane/zaktualizowane

---

##  Obowiązkowe Komentarze

### Backend (C#)
```csharp
/// <summary>
/// Oblicza TDEE (Total Daily Energy Expenditure) metodą Mifflin-St Jeor
/// Uwzględnia wiek, wagę, wzrost i poziom aktywności użytkownika
/// </summary>
/// <param name="user">Profil użytkownika z danymi antropometrycznymi</param>
/// <returns>Dzienne zapotrzebowanie kaloryczne w kcal</returns>
public decimal CalculateTdee(User user)
{
    // BMR dla mężczyzn: 10 * waga + 6.25 * wzrost - 5 * wiek + 5
    var bmr = (10 * user.Weight) + (6.25 * user.Height) - (5 * user.Age) + 5;
    
    // Mnożnik aktywności fizycznej (1.2 - 1.9)
    return bmr * user.ActivityLevel;
}
```

### Frontend (TypeScript)
```typescript
/**
 * Hook zarządza stanem dziennika diety z auto-refresh co 30s
 * Używa TanStack Query do cache i synchronizacji
 * 
 * @param date - Data dziennika (format: YYYY-MM-DD)
 * @returns Dane dziennika + loading state + error state
 */
export function useDiary(date: Date) {
  return useQuery({
    queryKey: ['diary', date],
    queryFn: () => diaryApi.getDaily(date),
    refetchInterval: 30000, // Auto-refresh dla real-time sync
  });
}
```

---

##  Zakazy

### Backend
```csharp
//  ZAKAZANE
public class user { }                    // Małe litery w klasach
public void DoSomething(dynamic data) { } // 'dynamic' bez powodu
catch (Exception) { }                     // Catch bez logowania
var result = _db.Query($"SELECT * FROM Users WHERE Id = {id}"); // SQL injection!

//  POPRAWNE
public class User { }
public void DoSomething(UserDto data) { }
catch (Exception ex) { _logger.LogError(ex, "Error"); }
var result = await _userRepository.GetByIdAsync(userId);
```

### Frontend
```typescript
//  ZAKAZANE
console.log('debug');                                    // Usuń przed commitem
<div dangerouslySetInnerHTML={{ __html: userInput }} />  // XSS!
const data: any = response;                              // Unikaj 'any'

//  POPRAWNE
// (bez console.log)
<div>{userInput}</div>                                   // React auto-escapes
const data: UserResponse = response;
```

---

##  Plan Iteracji

### Etap I (MVP) - **AKTUALNY**
-  Autoryzacja (JWT)
-  Profil użytkownika
-  Dziennik diety
-  Open Food Facts API

### Etap II (Analytics)
-  Pomiary ciała
-  Wykresy (waga, makroskładniki)
-  Redis cache

### Etap III (AI)
-  Claude API
-  Analiza zdjęć posiłków
-  AI Coach

 **Nie implementuj funkcji z przyszłych iteracji bez wyraźnego polecenia!**

---

##  Przykład Workflow
```
User: "Dodaj endpoint do logowania posiłków"

 GOOD Cline:
1. Czyta README.md → sprawdza Etap I
2. Czyta plantUML-kod.txt → rozumie warstwy
3. Szuka podobnego kodu: Ctrl+Shift+F "Command"
4. Tworzy:
   - FitApp.Application/Features/Diet/Commands/AddMealItemCommand.cs
   - AddMealItemCommandHandler.cs
   - AddMealItemValidator.cs
   - FitApp.API/Controllers/DiaryController.cs
5. Dodaje komentarze
6. Pyta: "Czy mam też stworzyć testy?"

 BAD Cline:
- Pisze kod bez czytania README/plantUML
- Tworzy pliki w złych folderach
- Brak komentarzy
- Narusza Clean Architecture
```

---

##  Komendy Pomocnicze

### Backend
```bash
# Build projektu
dotnet build

# Uruchom testy
dotnet test

# Uruchom API
dotnet run --project Backend/Backend.csproj
```

### Frontend
```bash
# Instalacja zależności
npm install

# Uruchom dev server
npm run dev

# Type check
npm run type-check

# Build produkcyjny
npm run build
```

---

##  Przydatne Linki

- **README.md** - Główna dokumentacja projektu
- **UML/plantUML-kod.txt** - Diagramy architektury
- **.clinerules/rules.md** - Szczegółowe zasady kodowania
- **.agents/skills/** - Dokumentacja poszczególnych umiejętności

---

##  Wsparcie

Jeśli Cline:
- Nie przestrzega zasad → Przypomnij: "Sprawdź .clinerules/rules.md"
- Pisze zbyt długo → Przypomnij: "Pisz jak neandertalczyk"
- Narusza Clean Architecture → Przypomnij: "Sprawdź UML/plantUML-kod.txt"

---

**Ostatnia aktualizacja:** 2026-04-28  
**Zespół:** FitApp Dev Team
