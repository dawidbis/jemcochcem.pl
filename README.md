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

| Metoda | Endpoint | Opis |
| :--- | :--- | :--- |
| `GET` | `/api/v1/diary/{date}` | Pobranie kompletnego dziennika z danego dnia. |
| `POST` | `/api/v1/diary/items` | Dodanie produktu do posiłku (ID, gramatura). |
| `GET` | `/api/v1/foods/barcode/{code}` | Pobranie danych produktu (z cache Redis). |
| `POST` | `/api/v1/profile/measurements` | Zapisanie nowej wagi lub obwodów ciała. |
| `POST` | `/api/v1/ai/chat` | Przesłanie zapytania do AI Coacha. |

---

## 📁 Struktura projektu

```text
FitApp/
├── FitApp.sln
├── docker-compose.yml
├── .gitignore
│
├── src/
│   ├── FitApp.Domain/              # Encje i reguły biznesowe
│   │   ├── Entities/               # User, MealLog, FoodProduct, BodyMeasurement
│   │   ├── ValueObjects/           # MacroNutrients, CalorieGoal
│   │   └── Interfaces/             # Abstrakcje repozytoriów
│   │
│   ├── FitApp.Application/         # Logika (CQRS + MediatR)
│   │   ├── Features/               # Diet, Profile, AI, Auth (Vertical Slices)
│   │   ├── DTOs/                   # Obiekty transferu danych
│   │   └── Validators/             # FluentValidation
│   │
│   ├── FitApp.Infrastructure/      # Implementacje techniczne
│   │   ├── Data/                   # AppDbContext, Migracje, Repositories
│   │   ├── ExternalServices/       # OpenFoodFactsClient, ClaudeAiService
│   │   └── Cache/                  # RedisCacheService (TTL 24h)
│   │
│   └── FitApp.API/                 # Prezentacja (Endpointy)
│       ├── Controllers/            # Auth, Diary, Profile, AI
│       ├── Middleware/             # ExceptionHandler, JwtMiddleware
│       └── Program.cs              # Konfiguracja DI
│
├── tests/                          # Testy jednostkowe i integracyjne
│
└── FitApp.Client/                  # Frontend (React + TS)
    ├── src/
    │   ├── api/                    # Instancje Axios, interceptory JWT
    │   ├── features/               # auth, diet, measurements, ai
    │   ├── components/             # Reusable UI (shadcn)
    │   ├── hooks/                  # Custom hooks (useAuth, useDebounce)
    │   └── App.tsx                 # Routing i Provider-y
