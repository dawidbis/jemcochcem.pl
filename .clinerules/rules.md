# 🤖 Zasady Cline — Projekt FitApp

## 🗣️ STYL KOMUNIKACJI

**Pisz jak neandertalczyk! Prosto, konkretnie, bez zbędnych słów.**

### Wzór komunikacji:
```
✅ DOBRZE:
"Ja robić endpoint. Dodać AddMealItemCommand. Testować dotnet build."

❌ ŹLE:
"Przystępuję do implementacji endpointu, który będzie obsługiwał dodawanie posiłków. 
W ramach tego zadania utworzę klasę AddMealItemCommand zgodnie z wzorcem CQRS..."
```

### Zasady:
- **Krótkie zdania** - maksymalnie 10 słów
- **Czasowniki w bezokoliczniku** - "robić", "dodać", "testować"
- **Konkretne akcje** - co robisz, gdzie, jak testujesz
- **Bez ozdobników** - żadnych "przystępuję", "w ramach", "zgodnie z"

---

## 📚 Obowiązkowa Lektura Przed Każdym Zadaniem

1. **`README.md`** — Tech stack, struktura projektu, iteracja (MVP/Analytics/AI)
2. **`UML/plantUML-kod.txt`** — Architektura Clean Architecture, flow danych

---

## ✅ Checklist

### Przed kodem:
- [ ] Przeczytaj `README.md` → sekcja "Stack technologiczny" + "Plan iteracji"
- [ ] Sprawdź `plantUML-kod.txt` → zrozum warstwę której dotykasz
- [ ] Wyszukaj podobny kod w projekcie (`Ctrl+Shift+F`)
- [ ] Sprawdź naming conventions (poniżej)

### Po kodzie:
- [ ] Backend: `dotnet build` przeszło bez błędów
- [ ] Frontend: `npm run type-check` przeszło
- [ ] Brak `console.log()` w kodzie
- [ ] **KOMENTARZE dodane** (CO, DLACZEGO, JAK)
- [ ] Testy napisane lub zaktualizowane

---

## 📁 Nazewnictwo Plików

### Backend (C#)
```
AddMealItemCommand.cs              // PascalCase + Command suffix
AddMealItemCommandHandler.cs       // PascalCase + Handler suffix
AddMealItemValidator.cs            // PascalCase + Validator suffix
IUserRepository.cs                 // Interface: I + PascalCase
UserRepository.cs                  // Implementacja bez I
```

**Struktura Backend (Clean Architecture):**
```
Backend/
├── FitApp.Domain/           # ⚠️ CORE - zero dependencies!
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
│   └── ExternalServices/    # OpenFoodFactsClient.cs, ClaudeAiService.cs
│
└── FitApp.API/             # Controllers (cienkie!)
    └── Controllers/         # DiaryController.cs
```

### Frontend (TypeScript/React)
```
MealLogItem.tsx                    // Komponenty: PascalCase
useAuth.ts                         // Hooki: camelCase + 'use' prefix
diary-api.ts                       // API clients: kebab-case
DiaryTypes.ts                      // Typy: PascalCase + Types suffix
```

**Struktura Frontend (Feature-based):**
```
frontend/src/
├── features/                # Vertical slices
│   ├── auth/
│   │   ├── components/      # LoginForm.tsx
│   │   ├── hooks/           # useAuth.ts
│   │   ├── api/             # auth-api.ts
│   │   └── types/           # AuthTypes.ts
│   │
│   └── diet/
│       ├── components/      # MealLogItem.tsx, DiaryView.tsx
│       ├── hooks/           # useDiary.ts
│       └── api/             # diary-api.ts
│
├── components/              # Shared (shadcn/ui)
│   └── ui/                  # Button.tsx, Input.tsx
│
└── lib/                     # Utils, config
    └── api-client.ts        # Axios config
```

---

## 🔧 Stack Technologiczny (z README.md)

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

## 💬 OBOWIĄZKOWE Komentarze

### Wzór:
```
// [CO] Opis zmiany
// [DLACZEGO] Powód wprowadzenia  
// [JAK] Mechanizm (jeśli złożony)
```

### Backend (C#):
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

### Frontend (TypeScript):
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

## 🚫 Zakazy

### Backend:
```csharp
// ❌ ZAKAZANE
public class user { }                                          // Małe litery w klasach
public void DoSomething(dynamic data) { }                      // 'dynamic' bez powodu
catch (Exception) { }                                          // Catch bez logowania
var result = _db.Query($"SELECT * FROM Users WHERE Id = {id}"); // SQL injection!

// ✅ POPRAWNE
public class User { }
public void DoSomething(UserDto data) { }
catch (Exception ex) { _logger.LogError(ex, "Error"); }
var result = await _userRepository.GetByIdAsync(userId);
```

### Frontend:
```typescript
// ❌ ZAKAZANE
console.log('debug');                                    // Usuń przed commitem
<div dangerouslySetInnerHTML={{ __html: userInput }} />  // XSS!
const data: any = response;                              // Unikaj 'any'

// ✅ POPRAWNE  
// (bez console.log)
<div>{userInput}</div>                                   // React auto-escapes
const data: UserResponse = response;
```

---

##  Iteracje (z README.md)

**Sprawdź w README sekcję "PLAN ITERACJI" przed implementacją!**

- **Etap I (MVP)**: Auth, Profile, Diary, OFF API
- **Etap II (Analytics)**: Body Measurements, Charts, Redis
- **Etap III (AI)**: Claude API, Photo Analysis, AI Coach

 Nie implementuj funkcji z przyszłych iteracji bez wyraźnego polecenia!

---

## 📝 Przykład właściwego workflow:

```
User: "Dodaj endpoint do logowania posiłków"

GOOD Cline:
1. Czyta README.md → sprawdza czy jesteśmy w Etapie I
2. Czyta plantUML-kod.txt → rozumie że to warstwa Application + API
3. Szuka podobnego kodu: Ctrl+Shift+F "Command" w FitApp.Application
4. Tworzy:
   - FitApp.Application/Features/Diet/Commands/AddMealItemCommand.cs
   - AddMealItemCommandHandler.cs
   - AddMealItemValidator.cs
   - FitApp.API/Controllers/DiaryController.cs (tylko endpoint)
5. Dodaje komentarze wyjaśniające logikę
6. Pyta: "Czy mam też stworzyć testy jednostkowe?"

 BAD Cline:
- Od razu pisze kod bez czytania README/plantUML
- Tworzy pliki w złych folderach
- Brak komentarzy
- Narusza Clean Architecture (np. Domain → Infrastructure reference)
```

---

**Ostatnia aktualizacja:** 2026-04-28  
**Zespół:** FitApp Dev Team