🍎 FitApp — Personalny Dziennik Diety i KaloriiLekka aplikacja fitness skoncentrowana na precyzyjnym liczeniu kalorii, zarządzaniu celami żywieniowymi oraz analizie składu ciała — zbudowana w React 18 + C# ASP.NET Core 8.📋 Spis treściOpis projektuAktorzy systemuStack technologicznyArchitekturaSchemat bazy danychModuły funkcjonalneREST APIStruktura projektuPlan iteracji

🎯 Opis projektuFitApp to darmowa platforma (Open Source / Projekt zaliczeniowy) służąca do monitorowania odżywiania. Skupia się na prostocie i dostarczaniu wartościowych danych o spożyciu bez zbędnych rozpraszaczy.ObszarOpisDziennik DietyLogowanie posiłków, kalkulator TDEE/BMR, skanowanie kodów kreskowych (Open Food Facts).Pomiary CiałaŚledzenie wagi, obwodów i poziomu tkanki tłuszczowej na osi czasu.AI SupportSzybka analiza kalorii ze zdjęcia talerza i konsultacja z AI Coachem (Claude API).

👥 Aktorzy systemu[User] — UżytkownikOblicza zapotrzebowanie kaloryczne (TDEE).Prowadzi codzienny dziennik posiłków (ręcznie lub skanerem).Analizuje postępy sylwetkowe na wykresach.Korzysta z analizy zdjęć posiłków przez AI.[System] — Zewnętrzne APIOpen Food Facts API — baza produktów spożywczych (barcode lookup).Claude API (Anthropic) — silnik AI do analizy zdjęć i porad dietetycznych.🛠 Stack technologicznyFrontendReact 18 (Vite) z TypeScript.TanStack Query do zarządzania stanem serwerowym.Tailwind CSS + shadcn/ui dla interfejsu.Recharts do wizualizacji wagi i makroskładników.BackendASP.NET Core 8.0 (C# 12).EF Core 8 + SQL Server (Baza danych).Redis jako cache dla wyszukiwań produktów (OFF API).MediatR (CQRS) dla czystej logiki biznesowej.JWT dla bezpiecznej autoryzacji (Access + Refresh Tokens).🗄 Schemat bazy danych (Uproszczony)Users: Profil, dane logowania.UserGoals: Aktualne cele (kcal, białko, węgle, tłuszcze).FoodProducts: Baza produktów (lokalna + zbuforowane dane z zewnątrz).MealLogs / MealLogItems: Rejestr spożycia z podziałem na posiłki i gramatury.BodyMeasurements: Historia wagi i wymiarów ciała.

📦 Moduły funkcjonalneKontrola DietyKalkulator Mifflin-St Jeor: Automatyczne wyznaczanie celu na start.Skaner kodów: Integracja z aparatem (React-barcode-qrcode-scanner).Baza produktów: Fallback z lokalnej bazy do Open Food Facts.Dzienny dashboard: Procentowa realizacja celów B/W/T.Moduł AIWizualna ocena posiłku: Zdjęcie → AI → Estymacja gramatury i kalorii.Chatbot: Możliwość zadawania pytań o zamienniki produktów lub porady dietetyczne.

🌐 REST APIMetodaEndpointOpisGET/api/v1/diary/{date}Kompletny dziennik dniaPOST/api/v1/diary/itemsDodaj produkt (gramatura, ID produktu)GET/api/v1/foods/barcode/{code}Pobierz dane po kodzie (z cache Redis)POST/api/v1/profile/measurementsZapisz nową wagę/obwodyPOST/api/v1/ai/chatPytanie do AI Coacha

📁 Struktura projektu FitApp
FitApp/
├── FitApp.sln
├── docker-compose.yml
├── .gitignore
│
├── src/
│   ├── FitApp.Domain/                  # Warstwa serca (Reguły biznesowe)
│   │   ├── Entities/                   # User, MealLog, MealLogItem, FoodProduct, BodyMeasurement
│   │   ├── ValueObjects/               # MacroNutrients (B/W/T), CalorieGoal
│   │   ├── Interfaces/                 # IUserRepository, IFoodRepository (abstrakcje)
│   │   └── Exceptions/                 # DomainException (np. przekroczenie zakresu kcal)
│   │
│   ├── FitApp.Application/             # Warstwa Logiki (CQRS + MediatR)
│   │   ├── Features/
│   │   │   ├── Diet/
│   │   │   │   ├── Commands/           # AddMealItem, UpdatePortion, DeleteMealItem
│   │   │   │   └── Queries/            # GetDailyDiary, GetFoodSearch, GetMacrosStats
│   │   │   ├── Profile/
│   │   │   │   ├── Commands/           # UpdateBodyMeasurements, CalculateTdee
│   │   │   │   └── Queries/            # GetWeightHistory
│   │   │   ├── AI/
│   │   │   │   └── Commands/           # AnalyzeMealPhoto, AskAiCoach
│   │   │   └── Auth/                   # Login, Register
│   │   ├── DTOs/                       # Obiekty transferu danych (UserDto, DiaryDto)
│   │   ├── Validators/                 # FluentValidation (np. gramatura musi być > 0)
│   │   └── Interfaces/                 # IClaudeAiService, IOffApiClient, IRedisCache
│   │
│   ├── FitApp.Infrastructure/          # Warstwa Techniczna (Implementacje)
│   │   ├── Data/
│   │   │   ├── AppDbContext.cs         # Konfiguracja EF Core
│   │   │   ├── Migrations/             # Migracje bazy SQL
│   │   │   └── Repositories/           # Konkretne implementacje dostępu do danych
│   │   ├── ExternalServices/
│   │   │   ├── OpenFoodFactsClient.cs  # Klient HTTP do bazy produktów
│   │   │   └── ClaudeAiService.cs      # Integracja z API Anthropic
│   │   └── Cache/
│   │       └── RedisCacheService.cs    # Przechowywanie wyników z OFF API (TTL 24h)
│   │
│   └── FitApp.API/                     # Warstwa Prezentacji (Endpointy)
│       ├── Controllers/
│       │   ├── AuthController.cs
│       │   ├── DiaryController.cs
│       │   ├── ProfileController.cs
│       │   └── AIController.cs
│       ├── Middleware/
│       │   ├── ExceptionHandler.cs     # Globalne wyłapywanie błędów
│       │   └── JwtMiddleware.cs        # Weryfikacja tokenów
│       └── Program.cs                  # Rejestracja DI i start aplikacji
│
├── tests/                              # Testy automatyczne
│   ├── FitApp.UnitTests/               # Logika domenowa i komendy MediatR
│   └── FitApp.IntegrationTests/        # Testy API (dostęp do bazy in-memory/test-container)
│
└── FitApp.Client/                      # Frontend (React + TS)
    ├── src/
    │   ├── api/                        # Axios instances (base URLs, interceptory JWT)
    │   ├── features/                   # Moduły funkcjonalne (Vertical Slices)
    │   │   ├── auth/                   # Login, Register, UserContext
    │   │   ├── diet/                   # Diary, MealLog, FoodSearch, BarcodeScanner
    │   │   ├── measurements/           # WeightChart, MeasurementForm
    │   │   └── ai/                     # PhotoUpload, AiChatWindow
    │   ├── components/                 # Współdzielone UI (Button, Input, Modal, Card)
    │   ├── hooks/                      # Custom hooks (useAuth, useLocalStorage, useDebounce)
    │   ├── types/                      # Interfejsy TypeScript (Food, User, Diary)
    │   ├── lib/                        # Konfiguracja (tailwind, shadcn, queryClient)
    │   ├── utils/                      # Helpery (formatowanie dat, przeliczanie makro)
    │   ├── App.tsx                     # Routing i Providey
    │   └── main.tsx                    # Entry point
    ├── public/                         # Zasoby statyczne (ikony, manifest PWA)
    ├── .env                            # Zmienne (VITE_API_URL)
    ├── tailwind.config.js
    └── vite.config.ts

📅 Plan iteracji (Core Edition)EtapZakresMVPAuth, Profil, TDEE, Dziennik posiłków, Skaner kodów.V2Pomiary ciała, Wykresy (Recharts), Cache Redis, Optymalizacja zapytań SQL.AI UpdateIntegracja z Claude API, Analiza zdjęć, Czat z trenerem AI.
