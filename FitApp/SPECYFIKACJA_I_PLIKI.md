# Specyfikacja Techniczna i Dokumentacja Modułu AI
## Projekt: FitApp (jemcochcem.pl)

Dokument opisuje architekturę, specyfikację funkcjonalną oraz wykaz plików zmodyfikowanych i utworzonych w ramach wdrożenia dwutorowego systemu generowania posiłków na podstawie makroskładników.

---

### 1. Precyzyjny Opis Funkcjonalności (Punkt 1 Specyfikacji)

**Nazwa modułu:** Dwutorowy, Inteligentny Generator Posiłków pod Makroskładniki z Integracją LLM.

**Opis działania:**
Moduł realizuje zadanie automatycznego dobierania składników i gramatur posiłków w oparciu o wprowadzone przez użytkownika docelowe wartości trzech głównych makroskładników: **Białka (g)**, **Węglowodanów (g)** oraz **Tłuszczów (g)**. 

W celu zapewnienia najwyższej stabilności systemu (High Availability) oraz unikalnych doświadczeń użytkownika (User Experience), system przetwarza zapytanie równolegle na dwóch niezależnych płaszczyznach:
1. **Klasyczny Algorytm (Deterministyczny):** Wykorzystuje sztywne, matematyczne reguły biznesowe zaszyte w systemie, które dobierają tradycyjne produkty z lokalnej bazy danych (np. jaja, chleb żytni, awokado) i precyzyjnie wyliczają ich masę. Stanowi on fundament stabilności aplikacji (wariant zapasowy/failsafe).
2. **Kreatywny Kreator AI (Niedeterministyczny):** Wykorzystuje integrację z zewnętrznym zaawansowanym modelem językowym **Google Gemini** (wersja `gemini-3.5-flash`) za pośrednictwem bezpośrednich połączeń HTTPS Web API. AI analizuje makroskładniki i generuje unikalne, zbilansowane przepisy kulinarne (np. łosoś w ziołach, komosa ryżowa), zwracając dane w restrykcyjnym formacie strukturalnym JSON.

Użytkownik otrzymuje na interfejsie graficznym bezpośrednie, czytelne porównanie obu wariantów w formie estetycznych paneli bocznych i może dokonać wyboru posiłku najlepiej odpowiadającego jego preferencjom.

---

### 2. Architektura i Wykaz Plików (Struktura Implementacji)

W ramach realizacji zadania wdrożono zmiany w warstwie prezentacji (Frontend - React) oraz w warstwie logiki i punktów dostępowych (Backend - ASP.NET Core Web API).

#### A. Warstwa Frontendu (Kliencka)
* **`src/Components/AiMealMatcher.tsx`**
    * *Typ:* Komponent React (TypeScript XML / TSX)
    * *Rola:* Odpowiada za kompletny Interfejs Użytkownika (UI). Zawiera formularz z walidacją pól liczbowych dla makroskładników, obsługuje stany asynchroniczne (loading/spinner) i renderuje dwa elastyczne panele wyników: ciemny dla algorytmu klasycznego i fioletowy dla sztucznej inteligencji. 
    * *Kluczowa logika:* Implementuje mechanizm obronny `try-catch` (fallback). W przypadku braku łączności z internetem, wygaśnięcia klucza API lub awarii zewnętrznych serwerów Google, automatycznie podstawia bezpieczny, z góry zdefiniowany zestaw danych, chroniąc aplikację przed wyświetleniem krytycznego błędu na ekranie. Zapobiega błędowi `NaNg` poprzez mapowanie zmiennej w formacie `weightInGrams`.
* **`package.json`**
    * *Typ:* Plik konfiguracyjny Node.js / NPM
    * *Rola:* Definiuje metadane projektu oraz zależności biblioteczne frontendu. Weryfikowano w nim obecność nowoczesnego stacku technologicznego (m.in. `tailwindcss` do stylizowania paneli, `lucide-react` do ikon systemowych oraz paczek `shadcn`), co pozwoliło na optymalne ostylowanie elementów interfejsu bez wgrywania nadmiarowych skryptów.

#### B. Warstwa Backendowa (Serwerowa)
* **`API/Controllers/AiMealController.cs`**
    * *Typ:* Klasa Kontrolera C# (ASP.NET Core Controller)
    * *Rola:* Wystawia publiczne punkty końcowe (REST API Endpoints) dla frontendu. Zarządza przepływem zapytań HTTP oraz komunikacją z usługami zewnętrznymi.
    * *Zaimplementowane Endpoints:*
        * `GET /api/AiMeal/generate` – Wykorzystuje architekturę CQRS i szynę danych `MediatR` do wysłania zapytania `GetAiMealQuery` do warstwy aplikacji, uruchamiając klasyczny algorytm.
        * `GET /api/AiMeal/generate-ai` – Odpowiada za bezpośrednią integrację z **Google Gemini API**. Bezpiecznie pobiera klucz autoryzacyjny ze środowiska systemowego, konstruuje rygorystyczny prompt inżynieryjny narzucający modelowi format JSON, realizuje asynchroniczne żądanie `POST` za pomocą `HttpClient` i samodzielnie oczyszcza surowy strumień tekstowy z ewentualnych znaczników markdown przed wysłaniem odpowiedzi do klienta.
* **`.env`**
    * *Typ:* Plik konfiguracyjny środowiska (Environment Variables)
    * *Rola:* Odizolowany plik konfiguracyjny przechowujący tajny token uwierzytelniający `GEMINI_API_KEY`. Zapobiega wyciekowi kluczy dostępowych do publicznego repozytorium kodu (Git), umożliwiając bezpieczne wykonywanie autoryzowanych zapytań do infrastruktury chmurowej Google bezpośrednio z lokalnego serwera deweloperskiego.

---

### 3. Schemat Przepływu Danych (Data Flow)