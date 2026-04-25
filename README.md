# 🏋️ FitApp — Platforma zarządzania siłownią i dietą

> Kompleksowa aplikacja fitness łącząca inteligentny licznik kalorii (inspirowany Fitatu), dziennik treningów siłowych, moduł siłowni z karnetami i rezerwacjami oraz AI Coacha — zbudowana w React 18 + C# ASP.NET Core 8.

![React](https://img.shields.io/badge/React-18-61DAFB?style=flat&logo=react&logoColor=white)
![TypeScript](https://img.shields.io/badge/TypeScript-5.x-3178C6?style=flat&logo=typescript&logoColor=white)
![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=flat&logo=dotnet&logoColor=white)
![SQL Server](https://img.shields.io/badge/SQL_Server-2022-CC2927?style=flat&logo=microsoftsqlserver&logoColor=white)
![Redis](https://img.shields.io/badge/Redis-7.x-DC382D?style=flat&logo=redis&logoColor=white)
![Docker](https://img.shields.io/badge/Docker-compose-2496ED?style=flat&logo=docker&logoColor=white)

---

## 📋 Spis treści

- [Opis projektu](#-opis-projektu)
- [Aktorzy systemu](#-aktorzy-systemu)
- [Stack technologiczny](#-stack-technologiczny)
- [Architektura](#-architektura)
- [Schemat bazy danych](#-schemat-bazy-danych)
- [Moduły funkcjonalne](#-moduły-funkcjonalne)
- [REST API](#-rest-api)
- [Struktura projektu](#-struktura-projektu)
- [Uruchomienie lokalne](#-uruchomienie-lokalne)
- [Zmienne środowiskowe](#-zmienne-środowiskowe)
- [Telegram Bot](#-telegram-bot)
- [Plan iteracji](#-plan-iteracji)
- [Wymagania niefunkcjonalne](#-wymagania-niefunkcjonalne)

---

## 🎯 Opis projektu

FitApp to kompleksowa platforma fitness dla siłowni i ich klientów. Łączy trzy obszary w jednej aplikacji:

| Obszar | Opis |
|--------|------|
| **Dieta** | Dziennik posiłków, kalkulator TDEE/BMR, skanowanie kodów kreskowych (Open Food Facts), AI liczenie kalorii ze zdjęcia |
| **Trening** | Dziennik treningów siłowych, progresja obciążeń, plany tygodniowe, kalkulator 1RM |
| **Siłownia** | Karnety z płatnościami Stripe, rezerwacje zajęć grupowych, QR check-in, dashboard analityczny admina |

**Przewaga nad Fitatu:** FitApp = dieta + treningi + moduł siłowni + AI Coach z historią użytkownika + Telegram Bot.

---

## 👥 Aktorzy systemu

System definiuje cztery role z pełnym RBAC (Role-Based Access Control) wymuszanym przez `[Authorize(Roles="...")]` w ASP.NET Core.

### `[User]` — Użytkownik / Sportowiec
- Konfiguruje profil i dzienny cel kaloryczny (TDEE oblicza backend)
- Loguje posiłki: ręcznie, skanując kod kreskowy lub robiąc zdjęcie talerza (AI)
- Loguje treningi siłowe: ćwiczenie → serie × powtórzenia × ciężar
- Śledzi postępy na wykresach (14 dni diety, progresja obciążeń)
- Kupuje karnet przez Stripe i rezerwuje zajęcia grupowe
- Rozmawia z AI Coachem przez web lub Telegram Bot

### `[Trainer]` — Trener personalny
- Wszystkie uprawnienia roli `[User]`
- Tworzy i zarządza planami treningowymi przypisanych klientów
- Przegląda dzienniki treningów i diety klientów
- Prowadzi zajęcia grupowe i widzi listę uczestników

### `[GymAdmin]` — Administrator siłowni
- Zarządza bazą członków i subskrypcjami (Stripe)
- Konfiguruje typy karnetów i cennik
- Tworzy grafik zajęć grupowych
- Skanuje QR kody przy check-in uczestników
- Widzi dashboard analityczny: przychody, MRR, retencja, churn

### `[System]` — Zewnętrzne API i procesy automatyczne
- **Stripe** — płatności, webhooki, Billing subskrypcje
- **Open Food Facts API** — baza produktów spożywczych (barcode lookup)
- **Claude API (Anthropic)** — AI Coach i liczenie kalorii ze zdjęcia
- **ExerciseDB API** — baza 1300+ ćwiczeń z animacjami GIF
- **Telegram Bot API** — komendy i push notyfikacje
- **SendGrid** — transakcyjne emaile
- **Hangfire** — cron jobs (daily summary, remindery, raporty)

### Macierz uprawnień RBAC

| Akcja | User | Trainer | GymAdmin | System |
|-------|:----:|:-------:|:--------:|:------:|
| Kalkulator TDEE + cele | ✅ | ✅ | ✅ | — |
| Dziennik posiłków (własny) | ✅ | ✅ | ✅ | — |
| Dziennik posiłków (klientów) | ❌ | ✅ | ✅ | — |
| Dziennik treningów (własny) | ✅ | ✅ | ✅ | — |
| Plany treningowe klientów | ❌ | ✅ | ✅ | — |
| Kupno karnetu | ✅ | ✅ | ❌ | — |
| Zarządzanie karnetami | ❌ | ❌ | ✅ | Stripe |
| Tworzenie grafiku zajęć | ❌ | ❌ | ✅ | — |
| Rezerwacja zajęć | ✅ | ✅ | ❌ | — |
| QR check-in (skanowanie) | ❌ | ❌ | ✅ | — |
| Dashboard analityczny | ❌ | Częściowo | ✅ | — |
| AI Coach | ✅ | ✅ | ❌ | — |
| Konfiguracja systemu | ❌ | ❌ | ✅ | — |

---

## 🛠 Stack technologiczny

### Frontend
| Technologia | Wersja | Zastosowanie |
|-------------|--------|--------------|
| React | 18 | SPA — główny framework UI |
| TypeScript | 5.x | Silne typowanie, bezpieczeństwo |
| Vite | 5.x | Build tool, HMR, tree-shaking |
| TanStack Query | 5.x | Cache, optimistic updates, data fetching |
| React Router | 6.x | Nested routes, lazy loading, AuthGuard |
| Tailwind CSS | 3.x | Utility-first CSS, zero runtime |
| shadcn/ui | latest | Dostępne komponenty UI |
| React Hook Form + Zod | latest | Formularze z walidacją schematową |
| Recharts + Chart.js | latest | Wykresy postępów i analityki |
| Axios | latest | HTTP client z interceptorami JWT |

### Backend
| Technologia | Wersja | Zastosowanie |
|-------------|--------|--------------|
| ASP.NET Core | 8.0 | REST API — Web API Controllers |
| C# | 12 | Język backendu |
| Clean Architecture | — | Domain / Application / Infrastructure / Presentation |
| MediatR | 12.x | CQRS — Commands i Queries |
| FluentValidation | 11.x | Pipeline behavior walidacji |
| Entity Framework Core | 8.x | ORM, Code-First migrations |
| SQL Server | 2022 | Główna baza danych (Azure SQL na prod) |
| Redis | 7.x | Cache OFF API (TTL 24h), Hangfire storage |
| Hangfire | 1.8 | Cron jobs: remindery, daily summary |
| ASP.NET Core Identity + JWT | 8.x | Auth, bcrypt, refresh token rotation |
| Stripe .NET SDK | latest | Płatności i subskrypcje |
| SendGrid SDK | latest | Emaile transakcyjne |
| Anthropic SDK (C#) | latest | Claude API dla AI Coacha |
| Swagger / OpenAPI | latest | Automatyczna dokumentacja API |

### Infrastruktura
| Technologia | Zastosowanie |
|-------------|--------------|
| Docker + docker-compose | Lokalne środowisko: api + frontend + db + redis |
| GitHub Actions | CI/CD: build → test → docker push → deploy |
| Azure App Service | Hosting API (autoscaling 2–10 instancji) |
| Azure SQL | Managed baza danych na produkcji |
| Azure Redis Cache | Managed Redis na produkcji |
| Application Insights | Traces, exceptions, custom metrics |
| Sentry | Error tracking frontendu React |

---

## 🏗 Architektura

### Uzasadnienie: Monolit modularny (nie mikroserwisy)

Jedno repo, jedna baza, jeden deployment — właściwy wybór dla projektu zaliczeniowego i fazy MVP. Czyste granice modułów umożliwiają późniejszą migrację do mikroserwisów.

### Clean Architecture — warstwy backendu

```
┌─────────────────────────────────────────────┐
│           Presentation (FitApp.API)          │
│   Controllers, Middleware, Swagger, CORS     │
├─────────────────────────────────────────────┤
│         Application (FitApp.Application)    │
│   CQRS Commands/Queries, DTOs, Validation   │
├─────────────────────────────────────────────┤
│        Infrastructure (FitApp.Infrastructure)│
│   EF Core, HTTP Clients, Hangfire, Redis     │
├─────────────────────────────────────────────┤
│            Domain (FitApp.Domain)           │
│   Entities, Value Objects, Interfaces       │
└─────────────────────────────────────────────┘
```

| Warstwa | Zawartość |
|---------|-----------|
| **Domain** | Encje, Value Objects, Domain Events, interfejsy repozytoriów — zero zależności zewnętrznych |
| **Application** | Use Cases (CQRS z MediatR), DTOs, walidacja (FluentValidation), interfejsy serwisów — zależy tylko od Domain |
| **Infrastructure** | EF Core DbContext, repozytoria, klienty HTTP (OFF, Claude, Stripe), Hangfire jobs, SendGrid — implementuje interfejsy Application |
| **Presentation** | ASP.NET Core Web API — Controllery, middleware JWT, Swagger, rate limiting — zależy od Application |

---

## 🗄 Schemat bazy danych

### Tabele główne

| Tabela | Kluczowe kolumny |
|--------|-----------------|
| `Users` | `Id UUID PK`, `Email UNIQUE`, `PasswordHash`, `Name`, `BirthDate`, `HeightCm`, `Sex`, `Role` |
| `UserGoals` | `UserId FK`, `WeightCurrentKg`, `WeightTargetKg`, `ActivityMultiplier`, `DailyKcalGoal`, `ProteinGGoal`, `CarbsGGoal`, `FatGGoal`, `GoalType` |
| `FoodProducts` | `Barcode IX`, `Name`, `Brand`, `KcalPer100g`, `ProteinPer100g`, `CarbsPer100g`, `FatPer100g`, `NutriScore`, `Source` ('off'\|'user') |
| `MealLogs` | `UserId FK`, `LogDate DATE IX`, `MealType` (breakfast\|lunch\|dinner\|snacks) |
| `MealLogItems` | `MealLogId FK`, `FoodProductId FK`, `QuantityG`, `Kcal`, `ProteinG`, `CarbsG`, `FatG` — **denormalizowane przy zapisie** |
| `WorkoutLogs` | `UserId FK`, `LogDate DATE IX`, `Notes`, `DurationMin`, `KcalBurnedEst` |
| `WorkoutLogSets` | `WorkoutLogId FK`, `ExerciseName`, `SetNumber`, `Reps`, `WeightKg`, `RestSec` |
| `BodyMeasurements` | `UserId FK`, `MeasuredAt DATE`, `WeightKg`, `BodyFatPct`, `WaistCm`, `ChestCm`, `HipsCm` |
| `GymMemberships` | `UserId FK`, `PlanType`, `Status`, `StartDate`, `EndDate`, `StripeSubscriptionId`, `PricePaid` |
| `GymClasses` | `Name`, `TrainerId FK`, `StartsAt`, `DurationMin`, `Capacity`, `Room` |
| `ClassBookings` | `ClassId FK`, `UserId FK`, `Status`, `QrToken UUID`, `CheckedInAt NULLABLE` |
| `AiConversations` | `UserId FK`, `Messages NVARCHAR(MAX)` — JSON array historii dla Claude API |

### Kluczowe decyzje

- **UUID PK** zamiast INT — brak przewidywalnych ID w URL, bezpieczne dla publicznego API
- **Denormalizacja makro** — `MealLogItems` przechowuje przeliczone kcal/makro przy zapisie → `GET /diary/{date}` to czysty `SUM()` bez runtime calculations
- **Indeksy** — obowiązkowe na: `MealLogs(UserId, LogDate)`, `WorkoutLogs(UserId, LogDate)`, `FoodProducts(Barcode)`, `BodyMeasurements(UserId, MeasuredAt)`
- **EF Core Code-First** — migracje wersjonowane, Owned Types dla Value Objects, Global Query Filters dla soft delete

---

## 📦 Moduły funkcjonalne

### Auth & zarządzanie kontem
| Funkcja | Opis | Priorytet | Aktor |
|---------|------|-----------|-------|
| Rejestracja email | Formularz + weryfikacja email SendGrid + bcrypt hash | **MVP** | User |
| Logowanie JWT | Access token 15min + Refresh token 30 dni (httpOnly cookie) | **MVP** | User |
| Role RBAC | User / Trainer / GymAdmin — `[Authorize(Roles=...)]` | **MVP** | System |
| Reset hasła | Email z tokenem jednorazowym, ważny 1h | **MVP** | User |
| Google OAuth | SSO przez Google — logowanie bez hasła | Etap 2 | User |

### Dieta & licznik kalorii
| Funkcja | Opis | Priorytet | Aktor |
|---------|------|-----------|-------|
| Kalkulator TDEE/BMR | Wzór Mifflin-St Jeor + mnożnik Harris-Benedict | **MVP** | User |
| Dziennik posiłków | 5 posiłków/dzień, nawigacja po dniach, real-time makro | **MVP** | User |
| Wyszukiwarka produktów | Lokalna baza SQL → fallback Open Food Facts API | **MVP** | User |
| Skaner kodów kreskowych | Barcode → OFF API + Redis cache 24h | **MVP** | User |
| Własna baza produktów | User dodaje brakujące produkty | **MVP** | User |
| AI liczenie ze zdjęcia | Claude Vision API: zdjęcie → JSON {składniki, kcal, makro} | Etap 2 | User / AI |
| Plany żywieniowe | Gotowe menu: Keto, Wegetariańskie, High-Protein, Low-Carb | Etap 2 | User |
| Statystyki tygodniowe | Wykres 14 dni, średnia kcal, dni w celu, trend makro | **MVP** | User |

### Trening siłowy
| Funkcja | Opis | Priorytet | Aktor |
|---------|------|-----------|-------|
| Dziennik treningów | Serie × powtórzenia × ciężar per ćwiczenie per dzień | **MVP** | User |
| Baza ćwiczeń | ExerciseDB API: 1300+ ćwiczeń z animacjami GIF | **MVP** | System |
| Plany treningowe | Trener lub AI tworzy plan; user wykonuje step-by-step | **MVP** | Trainer / AI |
| Progresja obciążeń | Wykres ciężar vs czas per ćwiczenie | Etap 2 | User |
| Kalkulator 1RM | Wzór Epley: `1RM = w × (1 + reps/30)` | Etap 2 | User |
| Notatki trenera | Trainer dodaje komentarze do sesji klienta | Etap 2 | Trainer |

### Moduł siłowni
| Funkcja | Opis | Priorytet | Aktor |
|---------|------|-----------|-------|
| Typy karnetów | Miesięczny / 3-mies. / roczny / jednorazowy — konfiguracja admina | **MVP** | GymAdmin |
| Stripe Checkout | Płatność kartą, BLIK, Apple Pay, Google Pay | **MVP** | User / Stripe |
| Auto-odnawianie | Stripe Billing + webhook → Hangfire job | **MVP** | System |
| Przypomnienia | Email 7 i 1 dzień przed końcem karnetu — Hangfire cron | **MVP** | System |
| Grafik zajęć | Admin tworzy zajęcia z limitem miejsc i trenerem | Etap 2 | GymAdmin |
| Rezerwacje | Max capacity + lista oczekujących z auto-awansowaniem | Etap 2 | User |
| QR check-in | User pokazuje QR, admin skanuje → POST `/bookings/{id}/checkin` | Etap 2 | User / Admin |
| Dashboard | Przychody, MRR, retencja, churn — wykresy Chart.js | Etap 2 | GymAdmin |

### AI Coach
| Funkcja | Opis | Priorytet |
|---------|------|-----------|
| Chatbot z historią | Claude API + kontekst 30 dni diety i treningów | Etap 2 |
| Generowanie planu | Tygodniowy jadłospis + plan treningowy na podstawie celu | Etap 2 |
| Analiza postępów | AI identyfikuje stagnację i sugeruje korekty | Etap 2 |
| Liczenie ze zdjęcia | Claude Vision: detect ingredients → kcal + macro per item | Etap 2 |
| Głosowe logowanie | Speech-to-text + AI parsing: *"zjadłem owsiankę z bananem"* | Etap 3 |

---

## 🌐 REST API

**Base URL:** `https://api.fitapp.pl/api/v1` (prod) | `https://localhost:5001/api/v1` (dev)

**Autoryzacja:** Bearer JWT w nagłówku `Authorization`. Refresh token jako httpOnly cookie.

**Format błędów:**
```json
{
  "statusCode": 400,
  "message": "Validation failed",
  "errors": ["Email is required", "Password must be at least 8 characters"]
}
```

### Auth
| Metoda | Endpoint | Opis | Auth |
|--------|----------|------|------|
| `POST` | `/auth/register` | Rejestracja konta | Public |
| `POST` | `/auth/login` | Logowanie → `{accessToken, user}` | Public |
| `POST` | `/auth/refresh` | Odśwież access token | Cookie RT |
| `POST` | `/auth/logout` | Unieważnij refresh token | `[User]` |
| `GET` | `/auth/me` | Dane zalogowanego użytkownika | `[User]` |
| `POST` | `/auth/forgot-password` | Wyślij email reset hasła | Public |
| `POST` | `/auth/reset-password` | Ustaw nowe hasło tokenem | Public |

### Profil & cele
| Metoda | Endpoint | Opis | Auth |
|--------|----------|------|------|
| `GET` | `/profile` | Profil + aktualne cele | `[User]` |
| `PUT` | `/profile` | Aktualizuj profil (waga, wzrost...) | `[User]` |
| `POST` | `/profile/goals` | Ustaw cel → backend oblicza TDEE + makro | `[User]` |
| `GET` | `/profile/goals` | Aktualne cele: kcal, białko, węgle, tłuszcze | `[User]` |
| `GET` | `/profile/measurements` | Historia pomiarów ciała | `[User]` |
| `POST` | `/profile/measurements` | Dodaj pomiar (waga, BF%, obwody) | `[User]` |

### Dieta
| Metoda | Endpoint | Opis | Auth |
|--------|----------|------|------|
| `GET` | `/diary/{date}` | Cały dzień: summary + 5 posiłków | `[User]` |
| `POST` | `/diary/{date}/meals/{type}/items` | Dodaj produkt do posiłku | `[User]` |
| `PUT` | `/diary/items/{id}` | Zmień gramaturę → przelicz makro | `[User]` |
| `DELETE` | `/diary/items/{id}` | Usuń produkt z posiłku | `[User]` |
| `GET` | `/diary/stats/weekly` | Statystyki 14 dni (dane do wykresów) | `[User]` |
| `GET` | `/foods/search?q=` | Szukaj produktów (local + OFF API) | `[User]` |
| `GET` | `/foods/barcode/{code}` | Pobierz po kodzie + Redis cache | `[User]` |
| `POST` | `/foods` | Dodaj własny produkt | `[User]` |
| `POST` | `/foods/analyze-photo` | Zdjęcie → AI → `{items, total_kcal, macros}` | `[User]` |

### Treningi
| Metoda | Endpoint | Opis | Auth |
|--------|----------|------|------|
| `GET` | `/workouts` | Lista treningów (`?date=`, `?userId=`) | `[User/Trainer]` |
| `POST` | `/workouts` | Utwórz log treningowy | `[User]` |
| `GET` | `/workouts/{id}` | Szczegóły treningu z seriami | `[User/Trainer]` |
| `POST` | `/workouts/{id}/sets` | Dodaj serię (ćwiczenie, ciężar, powt.) | `[User]` |
| `PUT` | `/workouts/sets/{id}` | Edytuj serię | `[User]` |
| `DELETE` | `/workouts/{id}` | Usuń trening | `[User]` |
| `GET` | `/exercises/search?q=` | Szukaj ćwiczeń (ExerciseDB + cache) | `[User]` |
| `GET` | `/workouts/progress/{exerciseName}` | Wykres progresji ciężaru | `[User/Trainer]` |

### Siłownia — karnety & zajęcia
| Metoda | Endpoint | Opis | Auth |
|--------|----------|------|------|
| `GET` | `/gym/plans` | Lista planów karnetów z cenami | Public |
| `POST` | `/gym/subscriptions` | Kup karnet → Stripe Checkout URL | `[User]` |
| `GET` | `/gym/subscriptions/me` | Aktualny karnet użytkownika | `[User]` |
| `DELETE` | `/gym/subscriptions/me` | Anuluj subskrypcję | `[User]` |
| `GET` | `/gym/classes` | Grafik zajęć z dostępnością | `[User]` |
| `POST` | `/gym/classes/{id}/book` | Zarezerwuj miejsce | `[User]` |
| `DELETE` | `/gym/bookings/{id}` | Anuluj rezerwację | `[User]` |
| `POST` | `/gym/checkin/{qrToken}` | QR check-in na zajęciach | `[GymAdmin]` |
| `POST` | `/webhooks/stripe` | Stripe webhook | Stripe-Signature |
| `GET` | `/admin/dashboard` | Analityka: przychody, retencja, churn | `[GymAdmin]` |

### AI Coach
| Metoda | Endpoint | Opis | Auth |
|--------|----------|------|------|
| `POST` | `/ai/chat` | Wyślij wiadomość → odpowiedź AI | `[User]` |
| `GET` | `/ai/chat/history` | Historia rozmów (ostatnie 50) | `[User]` |
| `DELETE` | `/ai/chat/history` | Wyczyść historię AI | `[User]` |
| `POST` | `/ai/generate-plan` | Generuj tygodniowy plan (dieta + trening) | `[User]` |

---

## 📁 Struktura projektu

```
FitApp/
├── FitApp.sln
│
├── src/
│   ├── FitApp.Domain/                  # Warstwa domenowa
│   │   ├── Entities/                   # User, MealLog, WorkoutLog, Membership...
│   │   ├── ValueObjects/               # MacroNutrients, CalorieGoal...
│   │   ├── Interfaces/                 # IUserRepository, IMealLogRepository...
│   │   └── Exceptions/                 # DomainException, NotFoundException...
│   │
│   ├── FitApp.Application/             # Warstwa aplikacji (CQRS)
│   │   ├── Features/
│   │   │   ├── Diet/
│   │   │   │   ├── Commands/           # AddMealItemCommand, CreateDiaryCommand...
│   │   │   │   └── Queries/            # GetDiaryQuery, GetWeeklyStatsQuery...
│   │   │   ├── Workout/
│   │   │   ├── Gym/
│   │   │   └── Auth/
│   │   ├── DTOs/
│   │   ├── Validators/                 # FluentValidation rules
│   │   └── Interfaces/                 # IStripeService, IEmailService, IAIService...
│   │
│   ├── FitApp.Infrastructure/          # Warstwa infrastruktury
│   │   ├── Data/
│   │   │   ├── AppDbContext.cs         # EF Core DbContext
│   │   │   ├── Migrations/
│   │   │   └── Repositories/
│   │   ├── ExternalServices/
│   │   │   ├── OpenFoodFactsClient.cs
│   │   │   ├── ClaudeAIService.cs
│   │   │   ├── StripeService.cs
│   │   │   ├── TelegramBotService.cs
│   │   │   └── SendGridEmailService.cs
│   │   └── BackgroundJobs/             # Hangfire jobs
│   │       ├── DailySummaryJob.cs
│   │       └── MembershipExpiryJob.cs
│   │
│   └── FitApp.API/                     # Warstwa prezentacji
│       ├── Controllers/
│       │   ├── AuthController.cs
│       │   ├── DiaryController.cs
│       │   ├── WorkoutController.cs
│       │   ├── GymController.cs
│       │   └── AIController.cs
│       ├── Middleware/
│       │   ├── JwtMiddleware.cs
│       │   └── ExceptionHandlingMiddleware.cs
│       └── Program.cs
│
├── tests/
│   ├── FitApp.Domain.Tests/
│   ├── FitApp.Application.Tests/
│   └── FitApp.API.Tests/               # Integration tests (WebApplicationFactory)
│
└── FitApp.Client/                      # React 18 + TypeScript
    ├── src/
    │   ├── features/
    │   │   ├── auth/
    │   │   │   ├── components/         # LoginForm, RegisterForm
    │   │   │   ├── hooks/              # useAuth, useJwt
    │   │   │   └── api/                # authApi.ts
    │   │   ├── diet/
    │   │   │   ├── components/         # DietDiary, MealCard, FoodSearch, BarcodeScanner
    │   │   │   ├── hooks/              # useDiary, useFoodSearch, useTdee
    │   │   │   └── api/                # dietApi.ts
    │   │   ├── workout/
    │   │   │   ├── components/         # WorkoutLog, ExercisePicker, ProgressChart
    │   │   │   ├── hooks/              # useWorkout, useExercises
    │   │   │   └── api/                # workoutApi.ts
    │   │   ├── gym/
    │   │   └── ai/
    │   ├── shared/
    │   │   ├── components/             # Button, Input, Card, Modal, Chart...
    │   │   ├── hooks/                  # useLocalStorage, useDebounce...
    │   │   └── types/                  # Global TypeScript types
    │   ├── lib/
    │   │   ├── axios.ts                # Axios instance + JWT interceptor
    │   │   └── queryClient.ts          # TanStack Query config
    │   └── App.tsx                     # Router + AuthProvider + QueryClientProvider
    ├── package.json
    └── vite.config.ts
```

---

## 🚀 Uruchomienie lokalne

### Wymagania

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Node.js 20+](https://nodejs.org/)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)

### 1. Sklonuj repozytorium

```bash
git clone https://github.com/your-username/fitapp.git
cd fitapp
```

### 2. Uruchom infrastrukturę (SQL Server + Redis)

```bash
docker-compose up -d
```

### 3. Backend — migracje i uruchomienie

```bash
cd src/FitApp.API
dotnet ef database update
dotnet run
# API dostępne na: https://localhost:5001
# Swagger: https://localhost:5001/swagger
```

### 4. Frontend

```bash
cd FitApp.Client
npm install
npm run dev
# App dostępna na: http://localhost:5173
```

### 5. docker-compose.yml

```yaml
services:
  sqlserver:
    image: mcr.microsoft.com/mssql/server:2022-latest
    environment:
      SA_PASSWORD: "FitApp_Dev_2025!"
      ACCEPT_EULA: "Y"
    ports:
      - "1433:1433"

  redis:
    image: redis:7-alpine
    ports:
      - "6379:6379"
```

---

## ⚙️ Zmienne środowiskowe

Utwórz plik `src/FitApp.API/appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=FitAppDev;User=sa;Password=FitApp_Dev_2025!;TrustServerCertificate=True",
    "Redis": "localhost:6379"
  },
  "Jwt": {
    "SecretKey": "your-super-secret-key-min-32-chars",
    "Issuer": "FitApp",
    "Audience": "FitAppClient",
    "AccessTokenExpiryMinutes": 15,
    "RefreshTokenExpiryDays": 30
  },
  "Stripe": {
    "SecretKey": "sk_test_...",
    "WebhookSecret": "whsec_..."
  },
  "Anthropic": {
    "ApiKey": "sk-ant-..."
  },
  "SendGrid": {
    "ApiKey": "SG...."
  },
  "Telegram": {
    "BotToken": "...",
    "WebhookUrl": "https://your-ngrok-url/api/v1/telegram/webhook"
  },
  "OpenFoodFacts": {
    "BaseUrl": "https://world.openfoodfacts.org"
  }
}
```

Frontend — plik `FitApp.Client/.env.local`:

```env
VITE_API_URL=https://localhost:5001/api/v1
VITE_STRIPE_PUBLIC_KEY=pk_test_...
```

---

## 🤖 Telegram Bot

Bot umożliwia logowanie posiłków i treningów bez otwierania aplikacji webowej.

### Komendy — dieta
| Komenda | Przykład | Opis |
|---------|---------|------|
| `/posilek` | `/posilek śniadanie owsianka 320kcal białko 12g` | Zaloguj posiłek |
| `/dzisiaj` | `/dzisiaj` | Podsumowanie dnia: kcal, makro |
| `/cel` | `/cel` | Sprawdź dzienny cel kaloryczny |
| `/tydzien` | `/tydzien` | Raport 7 dni — średnia kcal, dni w celu |
| `/kcal` | `/kcal banan 150g` | Szybkie sprawdzenie kalorii produktu |

### Komendy — trening
| Komenda | Przykład | Opis |
|---------|---------|------|
| `/trening` | `/trening wyciskanie 4x8 80kg` | Zaloguj trening |
| `/dzisiaj_trening` | `/dzisiaj_trening` | Treningi z dzisiaj |
| `/progresja` | `/progresja wyciskanie` | Wykres ciężarów |
| `/1rm` | `/1rm 80 8` | Kalkulator 1RM (Epley) |

### Powiadomienia automatyczne (Hangfire cron)
| Czas | Treść |
|------|-------|
| `08:00` | Dzień dobry! Dzienny cel: **{kcal} kcal**. Powodzenia! |
| `20:00` | Wieczorne podsumowanie — zjedzone kcal, makro, czy był trening |
| `Niedziela 18:00` | Tygodniowy raport — średnia kcal, treningi, zmiana wagi |

### Konfiguracja webhooka

```bash
curl -X POST "https://api.telegram.org/bot{TOKEN}/setWebhook" \
  -d "url=https://your-domain.com/api/v1/telegram/webhook"
```

---

## 📅 Plan iteracji

| Etap | Czas | Zakres |
|------|------|--------|
| **MVP** | Tyg. 1–3 | Auth (JWT), profil + TDEE, dziennik posiłków, OFF API barcode, karnety + Stripe, email, Telegram Bot (bazowy) |
| **Etap 2** | Tyg. 4–6 | Dziennik treningów, ExerciseDB, progresja, rezerwacje zajęć, QR check-in, dashboard admina, AI Coach |
| **Etap 3** | Tyg. 7–9 | AI generate plan, voice logging, export PDF, raporty finansowe, zaawansowane wykresy |
| **Polish** | Tyg. 10–12 | PWA offline, CI/CD GitHub Actions, load testing, >80% test coverage, security audit |

---

## 📊 Wymagania niefunkcjonalne

| Kategoria | Wymaganie |
|-----------|-----------|
| **Wydajność** | `GET /diary/{date}` < 200ms p95. Wyszukiwarka < 500ms z cache Redis. LCP strony < 1.5s. |
| **Skalowalność** | Stateless JWT → horyzontalne skalowanie API. Azure App Service autoscaling 2–10 instancji. |
| **Bezpieczeństwo** | HTTPS + HSTS. bcrypt rounds=12. JWT rotation. CORS whitelist. Rate limiting 100 req/min. EF Core parameterized queries (SQL injection prevention). |
| **Dostępność** | 99.5% uptime (Azure SLA). Automated daily backups Azure SQL. Health check: `GET /health`. |
| **Testy** | Unit: Domain + Application (>80% coverage, xUnit + Moq). Integration: API (WebApplicationFactory). E2E: Playwright dla critical flows. |
| **Observability** | Application Insights: traces, exceptions, custom metrics. Serilog + Seq. Sentry dla React. Alerty na p99 latency i error rate > 1%. |
| **GDPR** | Soft delete (`IsDeleted`). Export danych: `GET /profile/export`. Usunięcie konta: `DELETE /profile`. Szyfrowanie PII. |

---

## 📄 Licencja

MIT — projekt zaliczeniowy 2025.

---

*FitApp — React 18 + C# ASP.NET Core 8 + SQL Server | Specyfikacja techniczna v2.0*
