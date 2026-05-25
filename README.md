# 🍎 FitApp — Personalny Dziennik Diety i Kalorii

> Lekka aplikacja fitness skoncentrowana na precyzyjnym liczeniu kalorii, zarządzaniu celami żywieniowymi, monitorowaniu nawodnienia oraz analizie składu ciała — zbudowana w **React 18** + **C# ASP.NET Core 8**.

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
- [Instrukcja dla Developerów (Klucze API)](#-instrukcja-dla-developerów)
- [Plan iteracji](#-plan-iteracji)

---

## 🎯 Opis projektu

FitApp to darmowa platforma (Open Source / Projekt zaliczeniowy) służąca do kompleksowego monitorowania odżywiania. Skupia się na prostocie i dostarczaniu wartościowych danych o spożyciu bez zbędnych rozpraszaczy.

| Obszar | Opis |
| :--- | :--- |
| **Dziennik Diety** | Logowanie posiłków, kalkulator TDEE/BMR, skanowanie kodów kreskowych (Open Food Facts). |
| **Smart Water Tracker** | Algorytmiczne wyznaczanie celu nawodnienia na podstawie wagi oraz dynamiczne adaptowanie go do spożycia białka. |
| **Pomiary Ciała** | Śledzenie wagi, obwodów i poziomu tkanki tłuszczowej na osi czasu. |
| **AI Support (Gemini)** | Generowanie zbilansowanych planów dietetycznych oraz bezpośrednie dodawanie ich do dziennika dzięki wsparciu AI. |

---

## 👥 Aktorzy systemu

### `[User]` — Użytkownik
* Oblicza zapotrzebowanie kaloryczne (TDEE).
* Prowadzi codzienny dziennik posiłków (ręcznie, skanerem lub za pomocą asystenta AI).
* Monitoruje dzienne nawodnienie organizmu z opcją korygowania błędnych wpisów.
* Analizuje postępy sylwetkowe oraz trendy nawodnienia na interaktywnych wykresach długoterminowych.
* Generuje plany dietetyczne na podstawie własnych preferencji.

### `[System]` — Zewnętrzne API
* **Open Food Facts API** — globalna baza produktów spożywczych (barcode lookup).
* **Google Gemini API** — zaawansowany silnik AI do generowania spersonalizowanych planów posiłków i wyliczania makroskładników.

---

## 🛠 Stack technologiczny

### Frontend
| Technologia | Zastosowanie |
| :--- | :--- |
| **React 18 (Vite)** | Framework UI i środowisko uruchomieniowe. |
| **TypeScript** | Silne typowanie i bezpieczeństwo kodu. |
| **Tailwind CSS** | System stylizacji utility-first. |
| **shadcn/ui** | Biblioteka dostępnych komponentów UI. |
| **Recharts** | Wizualizacja trendów wagi, makroskładników oraz dziennego spożycia wody. |

### Backend
| Technologia | Zastosowanie |
| :--- | :--- |
| **ASP.NET Core 8.0** | Silnik REST API (C# 12). |
| **EF Core 8** | ORM do komunikacji z bazą danych PostgreSQL / SQL Server. |
| **Redis** | Szybki cache dla wyszukiwań produktów (OFF API). |
| **MediatR** | Implementacja wzorca CQRS dla czystej logiki biznesowej. |
| **JWT** | Bezpieczna autoryzacja (Access + Refresh Tokens). |

---

## 🗄 Schemat bazy danych (Uproszczony)

* **Users**: Profile użytkowników, dane logowania i preferencje.
* **UserGoals**: Aktualne cele (kcal, białko, węglowodany, tłuszcze).
* **FoodProducts**: Lokalna baza produktów + produkty dodane przez AI.
* **MealLogs / MealLogItems**: Rejestr dziennego spożycia z podziałem na posiłki.
* **WaterLogs**: Dzienny rejestr ilości wypitej wody powiązany z użytkownikiem i datą.
* **MealPlans / MealPlanItems**: Zapisane plany dietetyczne wygenerowane przez sztuczną inteligencję.
* **BodyMeasurements**: Historia wagi oraz precyzyjnych wymiarów ciała.

---

## 📦 Moduły funkcjonalne

### Kontrola Diety
* **Kalkulator Mifflin-St Jeor**: Automatyczne wyznaczanie celu kalorycznego na start.
* **Skaner kodów**: Integracja z aparatem telefonu (`React-barcode-qrcode-scanner`).
* **Baza produktów**: Mechanizm fallback (szukaj lokalnie -> szukaj w OFF API).
* **Dzienny dashboard**: Procentowa i wizualna realizacja celów B/W/T.

### Smart Water Tracker (Nowość!)
* **Dynamiczny cel dobowy**: Obliczanie zapotrzebowania bazowego na podstawie aktualnej wagi użytkownika ($Waga \times 35\text{ ml}$).
* **Algorytm ochrony nerek**: Automatyczne zwiększanie dobowego celu o $500\text{ ml}$ w przypadku wykrycia wysokiej podaży białka ($>140\text{ g}$) w dzienniku posiłków wraz z powiadomieniem w interfejsie.
* **Korekcja błędów (Human Error Handling)**: Możliwość cofania transakcji i wprowadzania korekt ujemnych (wsparcie transakcyjne po stronie Handlera).
* **Analityka trendów**: Integracja z kalendarzem długoterminowym oraz dedykowany wykres słupkowy w sekcji statystyk miesięcznych.

### Moduł AI & Planowanie
* **Generowanie Planów Dietetycznych**: Tworzenie zbilansowanych jadłospisów w formacie JSON na podstawie zapytania użytkownika (np. "dieta keto 2000 kcal").
* **Inteligentne Dodawanie (`AddAiDailyPlan`)**: Automatyczne przeniesienie wygenerowanego planu do właściwego Pamiętnika Posiłków wraz z dynamicznym przeliczaniem sum kalorycznych.

---

## 🌐 REST API

### 👤 Moduł: Users & Profile
| Metoda | Endpoint | Opis |
| :--- | :--- | :--- |
| `POST` | `/api/Users/register` | Rejestracja nowego użytkownika. |
| `POST` | `/api/Users/login` | Autoryzacja i pobranie tokenów JWT. |
| `POST` | `/api/Users/{id}/macros` | Obliczenie TDEE i celów makroskładników. |

### 🍎 Moduł: Foods (Baza Produktów)
| Method | Endpoint | Opis |
| :--- | :--- | :--- |
| `GET` | `/api/Foods/search` | Wyszukiwanie produktów (lokalne + OFF API). |
| `GET` | `/api/Foods/external/{barcode}` | Pobranie danych z Open Food Facts. |

### 📅 Moduł: Diary (Dziennik Posiłków)
| Metoda | Endpoint | Opis |
| :--- | :--- | :--- |
| `GET` | `/api/Diary/{userId}/{date}` | Pobranie podsumowania dnia z wyliczonym makro. |
| `POST` | `/api/Diary/items` | Ręczne dodanie produktu do dziennika. |
| `POST` | `/api/Diary/add-ai-daily-plan` | Dodanie wygenerowanego przez AI planu dnia prosto do dziennika. |

### 💧 Moduł: Water Tracker (MediatR CQRS — Nowość!)
| Metoda | Endpoint | Opis |
| :--- | :--- | :--- |
| `GET` | `/api/Water/status` | Pobiera stan nawodnienia oraz wyliczony algorytmicznie cel dobowy dla wybranej daty. |
| `POST` | `/api/Water/log` | Zapisuje lub koryguje (wartości ujemne) spożycie wody przez użytkownika. |

### 🤖 Moduł: AI Meal Plans
| Metoda | Endpoint | Opis |
| :--- | :--- | :--- |
| `POST` | `/api/MealPlans/generate` | Wysyła prompt do Google Gemini i generuje plan posiłków. |

---

## 📁 Struktura projektu (Czysta Architektura / Vertical Slices)

```text
FitApp/
├── src/
│   ├── FitApp.Domain/                  # Encje (WaterLog.cs), ValueObjects, Interfejsy
│   ├── FitApp.Application/             # Warstwa Logiki (CQRS + MediatR)
│   │   ├── Features/                   # Pionowe plastry (Vertical Slices)
│   │   │   ├── Diary/                  # GetDailyDiary, AddAiDailyPlanToDiary
│   │   │   ├── Foods/                  # SearchFoods, FetchExternal
│   │   │   ├── Water/                  # NOWOŚĆ: LogWaterIntakeCommand, GetWaterStatusQuery
│   │   │   └── AI_MealPlans/           # Moduł AI: GenerateMealPlan
│   │   ├── DTOs/                       # Obiekty transferu danych (np. WaterStatusDto)
│   │   └── Interfaces/                 # Abstrakcje serwisów i repozytoriów (IWaterLogRepository)
│   ├── FitApp.Infrastructure/          # Warstwa Techniczna (Implementacje)
│   │   ├── Data/                       # AppDbContext, Repositories (WaterLogRepository)
│   │   └── ExternalServices/           # GeminiAiService, OpenFoodFactsClient
│   └── FitApp.API/                     # Warstwa Prezentacji (REST Endpoints - WaterController)
├── tests/
│   └── FitApp.UnitTests/               # Testy jednostkowe xUnit + Moq (GetWaterStatusQueryHandlerTests.cs)
└── FitApp.Client/                      # Frontend (React 18 + TS + Vite)
    ├── src/
    │   ├── Components/                 # Komponenty UI (WaterTracker.tsx, DietCalendar.tsx)
    │   ├── api.ts                      # Konfiguracja warstwy integracji HTTP (fetch API)
    │   └── types.ts                    # Definicje typów TypeScript
