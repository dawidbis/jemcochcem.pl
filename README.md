# 🍎 FitApp — Personalny Dziennik Diety i Kalorii

> Lekka aplikacja fitness skoncentrowana na precyzyjnym liczeniu kalorii, zarządzaniu celami żywieniowymi oraz analizie składu ciała — zbudowana w **React 18** + **C# ASP.NET Core 8**.

![React](https://img.shields.io/badge/React-18-61DAFB?style=flat&logo=react&logoColor=white)
![TypeScript](https://img.shields.io/badge/TypeScript-5.x-3178C6?style=flat&logo=typescript&logoColor=white)
![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=flat&logo=dotnet&logoColor=white)
![SQL Server](https://img.shields.io/badge/SQL_Server-2022-CC2927?style=flat&logo=microsoftsqlserver&logoColor=white)
![Redis](https://img.shields.io/badge/Redis-7.x-DC382D?style=flat&logo=redis&logoColor=white)

---

## 📋 Spis treści

- [Opis projektu](#-opis-projektu)
- [Aktorzy systemu](#-aktorzy-systemu)
- [Stack technologiczny](#-stack-technologiczny)
- [Schemat bazy danych](#-schemat-bazy-danych)
- [Moduły funkcjonalne](#-moduły-funkcjonalne)
- [REST API](#-rest-api)
- [Struktura projektu](#-struktura-projektu)
- [Plan iteracji](#-plan-iteracji)

---

## 🎯 Opis projektu

FitApp to darmowa platforma (Open Source / Projekt zaliczeniowy) służąca do kompleksowego monitorowania odżywiania. Skupia się na prostocie i dostarczaniu wartościowych danych o spożyciu bez zbędnych rozpraszaczy.

| Obszar | Opis |
| :--- | :--- |
| **Dziennik Diety** | Logowanie posiłków, kalkulator TDEE/BMR, skanowanie kodów kreskowych (Open Food Facts). |
| **Pomiary Ciała** | Śledzenie wagi, obwodów i poziomu tkanki tłuszczowej na osi czasu. |
| **AI Support** | Szybka analiza kalorii ze zdjęcia talerza i konsultacja z AI Coachem (Claude API). |

---

## 👥 Aktorzy systemu

### `[User]` — Użytkownik
* Oblicza zapotrzebowanie kaloryczne (TDEE).
* Prowadzi codzienny dziennik posiłków (ręcznie lub skanerem).
* Analizuje postępy sylwetkowe na interaktywnych wykresach.
* Korzysta z analizy zdjęć posiłków przez AI.

### `[System]` — Zewnętrzne API
* **Open Food Facts API** — globalna baza produktów spożywczych (barcode lookup).
* **Claude API (Anthropic)** — zaawansowany silnik AI do analizy zdjęć i porad dietetycznych.

---

## 🛠 Stack technologiczny

### Frontend
| Technologia | Zastosowanie |
| :--- | :--- |
| **React 18 (Vite)** | Framework UI i środowisko uruchomieniowe. |
| **TypeScript** | Silne typowanie i bezpieczeństwo kodu. |
| **TanStack Query** | Zarządzanie stanem serwerowym i synchronizacja danych. |
| **Tailwind CSS** | System stylizacji utility-first. |
| **shadcn/ui** | Biblioteka dostępnych komponentów UI. |
| **Recharts** | Wizualizacja trendów wagi i makroskładników. |

### Backend
| Technologia | Zastosowanie |
| :--- | :--- |
| **ASP.NET Core 8.0** | Silnik REST API (C# 12). |
| **EF Core 8** | ORM do komunikacji z bazą danych SQL Server. |
| **Redis** | Szybki cache dla wyszukiwań produktów (OFF API). |
| **MediatR** | Implementacja wzorca CQRS dla czystej logiki biznesowej. |
| **JWT** | Bezpieczna autoryzacja (Access + Refresh Tokens). |

---

## 🗄 Schemat bazy danych (Uproszczony)

* **Users**: Profile użytkowników, dane logowania i preferencje.
* **UserGoals**: Aktualne cele (kcal, białko, węglowodany, tłuszcze).
* **FoodProducts**: Lokalna baza produktów + zbuforowane dane z OFF API.
* **MealLogs / MealLogItems**: Rejestr dziennego spożycia z podziałem na posiłki.
* **BodyMeasurements**: Historia wagi oraz precyzyjnych wymiarów ciała.

---

## 📦 Moduły funkcjonalne

### Kontrola Diety
* **Kalkulator Mifflin-St Jeor**: Automatyczne wyznaczanie celu kalorycznego na start.
* **Skaner kodów**: Integracja z aparatem telefonu (`React-barcode-qrcode-scanner`).
* **Baza produktów**: Mechanizm fallback (szukaj lokalnie -> szukaj w OFF API).
* **Dzienny dashboard**: Procentowa i wizualna realizacja celów B/W/T.

### Moduł AI
* **Wizualna ocena posiłku**: Przetworzenie zdjęcia przez AI w celu estymacji kalorii.
* **Chatbot**: Interaktywne pytania o zamienniki produktów lub porady dietetyczne.

---

## 🌐 REST API

### 👤 Moduł: Users & Profile
*Zarządzanie kontem, celami kalorycznymi i danymi biometrycznymi.*

| Metoda | Endpoint | Opis |
| :--- | :--- | :--- |
| `POST` | `/api/Users/register` | Rejestracja nowego użytkownika w systemie. |
| `POST` | `/api/Users/login` | Autoryzacja i pobranie podstawowych danych sesji. |
| `GET` | `/api/Users/{id}` | Pobranie danych profilowych (wiek, wzrost, płeć). |
| `PUT` | `/api/Users/{id}` | Aktualizacja danych profilu użytkownika. |
| `POST` | `/api/Users/{id}/macros` | Obliczenie TDEE i ustawienie celów makroskładników. |

---

### 🍎 Moduł: Foods (Baza Produktów)
*Zarządzanie produktami lokalnymi oraz integracja z zewnętrznymi bazami danych.*

| Metoda | Endpoint | Opis |
| :--- | :--- | :--- |
| `GET` | `/api/Foods/search` | Wyszukiwanie produktów (lokalne + query string). |
| `GET` | `/api/Foods/{id}` | Pobranie szczegółowych informacji o produkcie po ID. |
| `POST` | `/api/Foods` | Dodanie nowego, autorskiego produktu do lokalnej bazy. |
| `PUT` | `/api/Foods/{id}` | Edycja istniejącego produktu (korekta makroskładników). |
| `GET` | `/api/Foods/external/{barcode}` | Pobranie danych z Open Food Facts. |

---

### 📅 Moduł: Diary (Dziennik Posiłków)
*Operacje na dziennym spożyciu i logowanie posiłków.*

| Metoda | Endpoint | Opis |
| :--- | :--- | :--- |
| `GET` | `/api/Diary/{userId}/{date}` | Pobranie wpisów i podsumowania makro z danego dnia. |
| `POST` | `/api/Diary/items` | Dodanie produktu do wybranego posiłku (np. Śniadanie). |
| `POST` | `/api/Diary/items/barcode` | Szybkie dodanie produktu do dziennika via skaner kodów. |
| `PUT` | `/api/Diary/items/{id}` | Zmiana ilości (gramatury) dodanej pozycji. |
| `DELETE` | `/api/Diary/items/{id}` | Usunięcie produktu z dziennika posiłków. |

---

### 📉 Moduł: Measurements (Postępy)
*Monitorowanie zmian masy ciała i wymiarów.*

| Metoda | Endpoint | Opis |
| :--- | :--- | :--- |
| `POST` | `/api/Measurements` | Zapisanie nowego pomiaru wagi/ciała. |
| `GET` | `/api/Measurements/user/{userId}` | Pobranie pełnej historii pomiarów dla wykresów. |
| `DELETE` | `/api/Measurements/{id}` | Usunięcie błędnego wpisu historycznego. |

---

### 🤖 Moduł: AI Support
*Interakcja z modelem językowym Claude.*

| Metoda | Endpoint | Opis |
| :--- | :--- | :--- |
| `POST` | `/api/AI/analyze-photo` | Estymacja wartości odżywczych na podstawie zdjęcia. |
| `POST` | `/api/AI/chat` | Konsultacja z AI Coachem w kontekście diety. |

---

## 📁 Struktura projektu

```text
FitApp/
├── FitApp.sln
├── docker-compose.yml
├── .gitignore
│
├── src/
│   ├── FitApp.Domain/                  # Warstwa serca (Domain Driven Design)
│   │   ├── Entities/                   # User, MealLog, MealLogItem, FoodProduct, BodyMeasurement
│   │   ├── ValueObjects/               # MacroNutrients (B/W/T), CalorieGoal
│   │   ├── Interfaces/                 # Abstrakcje: IUserRepository, IFoodRepository, IBodyMeasurementRepository
│   │   └── Exceptions/                 # DomainException (specyficzne błędy biznesowe)
│   │
│   ├── FitApp.Application/             # Warstwa Logiki (CQRS + MediatR)
│   │   ├── Features/                   # Pionowe plastry (Vertical Slices) funkcjonalności
│   │   │   ├── Diary/                  # Dziennik: AddMealItem, DeleteMealItem, GetDailyDiary
│   │   │   ├── Foods/                  # Baza produktów: CreateFood, UpdateFood, SearchFoods,FetchExternal
│   │   │   ├── Measurements/           # Postępy: AddMeasurement, DeleteMeasurement, GetHistory
│   │   │   ├── Users/                  # Profil i Cele: UpdateProfile, CalculateMacros, GetUserProfile
│   │   │   ├── AI/                     # Moduł AI: AnalyzeMealPhoto, AskAiCoach
│   │   │   └── Auth/                   # Autoryzacja: Login, Register
│   │   ├── DTOs/                       # Obiekty transferu danych (UserDto, DiaryDto, FoodDto, MeasurementDto)
│   │   ├── Validators/                 # FluentValidation (reguły walidacji komend i zapytań)
│   │   └── Interfaces/                 # Abstrakcje serwisów: IClaudeAiService, IOffApiClient, IRedisCache
│   │
│   ├── FitApp.Infrastructure/          # Warstwa Techniczna (Implementacje)
│   │   ├── Data/
│   │   │   ├── AppDbContext.cs         # Konfiguracja EF Core i Fluent API
│   │   │   ├── Migrations/             # Migracje bazy danych SQL Server
│   │   │   └── Repositories/           # Konkretne implementacje dostępu do danych
│   │   ├── ExternalServices/
│   │   │   ├── OpenFoodFactsClient.cs  # Klient API zewnętrznej bazy produktów
│   │   │   └── ClaudeAiService.cs      # Integracja z Anthropic Claude API
│   │   └── Cache/
│   │       └── RedisCacheService.cs    # Buforowanie wyników (TTL 24h)
│   │
│   └── FitApp.API/                     # Warstwa Prezentacji (REST Endpoints)
│       ├── Controllers/                # UsersController, FoodsController, DiaryController, MeasurementsController
│       ├── Middleware/                 # ExceptionHandler (globalny), JwtMiddleware (auth)
│       └── Program.cs                  # Konfiguracja kontenera DI i potoku HTTP
│
├── tests/                              # Testy automatyczne
│   ├── FitApp.UnitTests/               # Testy jednostkowe logiki biznesowej i encji
│   └── FitApp.IntegrationTests/        # Testy integracyjne endpointów API
│
└── FitApp.Client/                      # Frontend (React 18 + TS + Vite)
    ├── src/
    │   ├── api/                        # Konfiguracja Axios i interceptory JWT
    │   ├── features/                   # Moduły UI (auth, diet, measurements, ai)
    │   ├── components/                 # Współdzielone UI (shadcn/ui)
    │   ├── hooks/                      # Customowe hooki (useAuth, useMeasurements)
    │   ├── types/                      # Wspólne interfejsy TypeScript (odpowiedniki DTO)
    │   ├── lib/                        # Konfiguracja TanStack Query i Tailwind
    │   ├── App.tsx                     # Główny router aplikacji
    │   └── main.tsx                    # Punkt wejścia
    └── .env                            # Zmienne środowiskowe (API_URL)
-------------------------------------------------------------------------
PLAN ITERACJI (Core Edition)
-------------------------------------------------------------------------

Etap I: MVP (Minimum Viable Product)
- System autoryzacji (JWT: Register/Login).
- Profil użytkownika + automatyczny kalkulator zapotrzebowania TDEE.
- Podstawowy dziennik posiłków (dodawanie/usuwanie pozycji).
- Integracja z Open Food Facts (wyszukiwanie produktów + skaner kodów).

Etap II: Analityka i Cache
- Moduł pomiarów ciała (waga, obwody) z historią.
- Wizualizacja postępów na wykresach (Recharts).
- Wdrożenie Redis Cache dla wyników wyszukiwania produktów.
- Optymalizacja bazy danych (indeksy pod daty i UserId).

Etap III: AI Update
- Integracja z Claude AI API (Anthropic).
- Moduł "Analyze Photo" (estymacja kalorii na podstawie przesłanego zdjęcia).
- Czat z AI Coachem (porady dietetyczne w oparciu o kontekst użytkownika).
- Eksport raportów tygodniowych do formatu tekstowego/PDF.
