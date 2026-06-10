# FitApp — Personalny Dziennik Diety i Siłowni

> Kompleksowa aplikacja fitness do śledzenia diety, treningów i składu ciała — zbudowana w **React 18 + TypeScript** i **ASP.NET Core 8**.

![React](https://img.shields.io/badge/React-18-61DAFB?style=flat&logo=react&logoColor=white)
![TypeScript](https://img.shields.io/badge/TypeScript-5.x-3178C6?style=flat&logo=typescript&logoColor=white)
![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=flat&logo=dotnet&logoColor=white)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-15-336791?style=flat&logo=postgresql&logoColor=white)
![Redis](https://img.shields.io/badge/Redis-7.x-DC382D?style=flat&logo=redis&logoColor=white)

---

## Spis treści

- [Opis projektu](#opis-projektu)
- [Funkcje](#funkcje)
- [Stack technologiczny](#stack-technologiczny)
- [Architektura](#architektura)
- [Schemat bazy danych](#schemat-bazy-danych)
- [Uruchomienie](#uruchomienie)
- [Zmienne środowiskowe](#zmienne-środowiskowe)
- [REST API](#rest-api)
- [Struktura projektu](#struktura-projektu)
- [Testy](#testy)

---

## Opis projektu

FitApp to open-source'owa platforma fitness oferująca precyzyjne liczenie kalorii, zarządzanie celami żywieniowymi, monitorowanie nawodnienia, analizę składu ciała oraz kompleksowy moduł treningowy z obliczaniem progresji.

### Aktorzy systemu

**Użytkownik** — oblicza TDEE, prowadzi dziennik posiłków, monitoruje nawodnienie, analizuje postępy sylwetkowe, loguje treningi i śledzi progresję siłową.

**Zewnętrzne API** — Open Food Facts (baza produktów), Google Gemini (generowanie planów diety i dobór posiłków AI).

---

## Funkcje

### Żywienie

| Moduł | Co robi |
|---|---|
| **Dziennik posiłków** | Dodawanie produktów po nazwie lub kodzie kreskowym (OpenFoodFacts API), kalorie + makroskładniki + mikroskładniki (błonnik, sód, wapń...) |
| **Kalkulator TDEE** | Wylicza zapotrzebowanie kaloryczne i cele makro/mikro na podstawie profilu (Mifflin-St Jeor) |
| **Kalendarz diety** | Widok miesięczny, wykresy kalorii / makroskładników / nawodnienia, szczegóły dnia |
| **Generator planu AI** | Gemini AI generuje plan posiłków z prompta użytkownika, jednym kliknięciem przenosi do dziennika |
| **Dobór posiłku AI** | Klasyczne + AI wyszukiwanie posiłku dopasowanego do podanych makro (B/W/T) |
| **Nawodnienie** | Kafelek z celem (+250/+500 ml), algorytm korekty celu przy wysokim spożyciu białka (>140 g → +500 ml), cofanie wpisów |
| **Streak diety** | Licznik aktywnych dni, rekord, pasek w dzienniku |

### Ciało

| Moduł | Co robi |
|---|---|
| **Pomiary ciała** | Waga, BMI (wskaźnik + wykres + gauge), % tkanki tłuszczowej, talia, biodra, cel wagowy z paskiem postępu |

### Siłownia

| Zakładka | Co robi |
|---|---|
| **Trening** | Logger serii (ćwiczenie → ciężar → powt.), podpowiedź optymalnego ciężaru z poprzedniego treningu, zapis sesji z notatką i czasem |
| **Historia** | Chronologiczna lista sesji z tonażem i listą serii, usuwanie sesji |
| **Progresja** | Wykres szacowanego 1RM (wzór Epleya) i tonażu dla wybranego ćwiczenia, tabela historii, automatyczna sugestia na następny trening |
| **Kalendarz** | Miesięczny widok aktywności, szczegóły sesji po kliknięciu dnia |
| **Ćwiczenia** | Dodaj/usuń własne ćwiczenia; lista 16 globalnych pogrupowanych mięśniowo |

### Konto

| Funkcja | Opis |
|---|---|
| **Autentykacja** | Rejestracja i logowanie z JWT (access token 15 min + refresh token 7 dni, auto-refresh w tle) |
| **Profil** | Waga, wzrost, wiek, płeć, mnożnik aktywności |

---

## Stack technologiczny

### Frontend

| Technologia | Zastosowanie |
|---|---|
| **React 18 + Vite** | Framework UI i środowisko dev |
| **TypeScript** | Silne typowanie |
| **Tailwind CSS** | Stylizacja utility-first |
| **shadcn/ui** | Komponenty UI (Button, Input, Sidebar…) |
| **Recharts** | Wykresy (waga, makro, 1RM, tonaż, woda) |
| **lucide-react** | Ikony |

### Backend

| Technologia | Zastosowanie |
|---|---|
| **ASP.NET Core 8** | REST API |
| **Entity Framework Core 8** | ORM, migracje (PostgreSQL) |
| **MediatR** | CQRS — Commands & Queries |
| **PostgreSQL 15** | Główna baza danych |
| **Redis** | Cache dla wyszukiwań produktów |
| **JWT Bearer + BCrypt** | Autentykacja i haszowanie haseł |
| **Google Gemini API** | Generowanie planów diety |
| **DotNetEnv** | Wczytywanie `.env` |

---

## Architektura

Backend oparty na Clean Architecture z wzorcem CQRS (MediatR):

```
FitApp/
├── API/Controllers/        — REST endpoints (dziedziczą ApiControllerBase → CurrentUserId z JWT)
├── Application/Features/   — Commands & Queries (MediatR)
│   ├── Diary/
│   ├── Foods/
│   ├── Measurements/
│   ├── Users/
│   ├── Workouts/
│   └── GenerateMealPlan/
├── Domain/                 — Encje, Value Objects, serwisy domenowe
│   ├── Exercise.cs, WorkoutSession.cs, SetEntry.cs
│   ├── User.cs, FoodProduct.cs, MealLog.cs, BodyMeasurement.cs, WaterLog.cs
│   └── Services/  (WorkoutCalculationService, TdeeCalculationService, GeminiService…)
├── Infrastructure/
│   ├── Data/               — AppDbContext, EF migrations, repozytoria
│   └── Interfaces/         — IWorkoutRepository, IMealPlanRepository…
└── Migrations/             — EF Core migrations (PostgreSQL)
```

**Ścieżka żądania:**
`Controller` → `IMediator.Send(Command/Query)` → `Handler` → `Repository` → `PostgreSQL`

Wszystkie chronione endpointy dziedziczą z `ApiControllerBase` z atrybutem `[Authorize]` — `CurrentUserId` jest pobierany z tokenu JWT, nie z URL.

---

## Schemat bazy danych

| Tabela | Kluczowe kolumny |
|---|---|
| `Users` | Id, Email, PasswordHash, Weight, Height, Age, Gender, TargetWeight, ActivityMultiplier, RefreshToken, CurrentStreak |
| `FoodProducts` | Id, Name, Barcode, CaloriesPer100g, makro + mikro per 100 g |
| `MealLogs` | Id, UserId, Date |
| `MealLogItems` | Id, MealLogId, FoodProductId, Grams |
| `MealPlans` | Id, UserId, Prompt, CreatedAt |
| `MealPlanItems` | Id, MealPlanId, ProductName, MealType, Grams, kalorie, makro |
| `BodyMeasurements` | Id, UserId, Date, Weight, BodyFatPercentage, Waist, Hips, BMI |
| `WaterLogs` | Id, UserId, Date, AmountMl |
| `Exercises` | Id, Name, MuscleGroup, IsCustom, UserId (null = globalne) |
| `WorkoutSessions` | Id, UserId, Date, Notes, DurationMinutes |
| `SetEntries` | Id, WorkoutSessionId, ExerciseId, SetNumber, Weight, Reps |

---

## Uruchomienie

### Docker Compose (zalecane)

**Wymagania:** Docker Desktop

```bash
# 1. Sklonuj repo
git clone https://github.com/dawidbis/jemcochcem.pl.git
cd jemcochcem.pl

# 2. Skonfiguruj zmienne środowiskowe
cp FitApp/.env.example FitApp/.env
# Uzupełnij GEMINI_API_KEY w FitApp/.env

# 3. Uruchom
docker compose up --build
```

Aplikacja dostępna pod `http://localhost:8080`.

### Lokalnie (bez Dockera)

**Wymagania:** .NET 8 SDK, Node.js 20+, PostgreSQL, Redis

```bash
# Backend
cp FitApp/.env.example FitApp/.env
# Uzupełnij klucz Gemini i connection string

cd FitApp
dotnet run
# API: http://localhost:5000
# Swagger: http://localhost:5000/swagger  (tylko w Development)

# Frontend (oddzielne okno terminala)
cd FitApp.Client
npm install
npm run dev
# Vite dev server: http://localhost:5173
```

---

## Zmienne środowiskowe

### `FitApp/.env` (nie jest commitowany)

```env
GEMINI_API_KEY=twój_klucz_google_gemini
```

Wygeneruj klucz na: https://aistudio.google.com/apikey

### `FitApp/appsettings.json`

| Klucz | Opis | Domyślnie |
|---|---|---|
| `ConnectionStrings:DefaultConnection` | Connection string PostgreSQL | localhost dev |
| `ConnectionStrings:Redis` | Host:Port Redis | `redis:6379` |
| `Jwt:Key` | Sekret JWT (min. 32 znaki) | ustawiony w pliku |
| `Jwt:Issuer` | Issuer tokenu | `FitApp` |
| `Jwt:Audience` | Audience tokenu | `FitAppClient` |

---

## REST API

Swagger dostępny w Development pod `/swagger`.

### Autentykacja

| Metoda | Endpoint | Opis |
|---|---|---|
| `POST` | `/api/users/register` | Rejestracja → zwraca accessToken, refreshToken |
| `POST` | `/api/users/login` | Logowanie → zwraca accessToken, refreshToken, userId |
| `POST` | `/api/users/refresh` | Odśwież token (body: `{userId, refreshToken}`) |

### Profil

| Metoda | Endpoint | Opis |
|---|---|---|
| `GET` | `/api/users/{id}` | Pobierz profil |
| `PUT` | `/api/users/{id}` | Aktualizuj profil |
| `POST` | `/api/users/me/macros` | Oblicz TDEE i cele makro |
| `POST` | `/api/users/me/micros` | Oblicz cele mikroskładników |
| `PUT` | `/api/users/me/target-weight` | Ustaw cel wagowy |

### Dziennik i produkty

| Metoda | Endpoint | Opis |
|---|---|---|
| `GET` | `/api/diary/{date}` | Podsumowanie dnia |
| `POST` | `/api/diary/items` | Dodaj pozycję do dziennika |
| `DELETE` | `/api/diary/{date}/items/{id}` | Usuń pozycję |
| `POST` | `/api/diary/items/from-ai-plan` | Dodaj z planu AI |
| `GET` | `/api/foods/search?query=` | Wyszukaj produkt |
| `GET` | `/api/foods/external/{barcode}` | OpenFoodFacts lookup |
| `POST` | `/api/foods` | Zapisz produkt lokalnie |

### Nawodnienie

| Metoda | Endpoint | Opis |
|---|---|---|
| `GET` | `/api/water/status?date=` | Status nawodnienia + cel |
| `POST` | `/api/water/log` | Zaloguj wodę (ujemna wartość = cofnięcie) |

### Pomiary ciała

| Metoda | Endpoint | Opis |
|---|---|---|
| `POST` | `/api/measurements` | Dodaj pomiar |
| `GET` | `/api/measurements/me` | Historia pomiarów |
| `GET` | `/api/measurements/me/stats` | Statystyki (BMI, postęp do celu) |
| `DELETE` | `/api/measurements/{id}` | Usuń pomiar |

### Plany diety AI

| Metoda | Endpoint | Opis |
|---|---|---|
| `POST` | `/api/mealplans/generate` | Generuj plan (body: `{prompt}`) |
| `GET` | `/api/mealplans` | Pobierz plany użytkownika |

### Siłownia

| Metoda | Endpoint | Opis |
|---|---|---|
| `GET` | `/api/workouts/exercises` | Lista ćwiczeń (globalne + własne) |
| `POST` | `/api/workouts/exercises` | Dodaj własne ćwiczenie |
| `DELETE` | `/api/workouts/exercises/{id}` | Usuń własne ćwiczenie |
| `GET` | `/api/workouts/exercises/{id}/progression` | Progresja + sugestia |
| `GET` | `/api/workouts/sessions` | Historia sesji (`?from=&to=`) |
| `POST` | `/api/workouts/sessions` | Zapisz sesję treningową |
| `DELETE` | `/api/workouts/sessions/{id}` | Usuń sesję |

---

## Struktura projektu

```
jemcochcem.pl/
├── FitApp/                         — Backend ASP.NET Core
│   ├── API/Controllers/            — REST controllers
│   ├── Application/Features/       — CQRS (Commands + Queries + Handlers)
│   ├── Domain/                     — Encje i serwisy domenowe
│   ├── Infrastructure/             — EF Core, repozytoria, zewnętrzne serwisy
│   ├── Migrations/                 — EF Core migrations
│   ├── appsettings.json
│   └── .env                        — Klucze API (nie w repo — patrz .env.example)
├── FitApp.Client/                  — Frontend React/TypeScript
│   └── src/
│       ├── Components/             — WorkoutHub, FoodDiary, BodyMeasurements…
│       ├── api.ts                  — Klient HTTP (authFetch + auto-refresh JWT)
│       └── types.ts                — Typy TypeScript
├── FitApp.Tests/                   — Testy jednostkowe (xUnit)
├── docker-compose.yml              — PostgreSQL + Redis + API
├── Dockerfile                      — Multi-stage build (React → .NET)
└── FitApp/.env.example             — Szablon zmiennych środowiskowych
```

---

## Testy

```bash
cd FitApp.Tests
dotnet test
```

Pokrycie: `WorkoutCalculationService` — obliczenia tonażu sesji i szacowanego 1RM (wzór Epleya).
