# ADR 001: Wdrożenie zewnętrznego modelu LLM (Google Gemini) dla modułu generowania posiłków

**Data:** 6 czerwca 2026
**Status:** Zaakceptowany
**Kontekst:** Projekt FitApp (jemcochcem.pl)

## 1. Kontekst i problem
W ramach rozwoju aplikacji FitApp zaszła potrzeba stworzenia inteligentnego kreatora przepisów kulinarnych dopasowanych do ścisłych wymogów makroskładnikowych użytkownika. Głównym problemem projektowym był wybór pomiędzy stworzeniem i utrzymywaniem własnej, gigantycznej bazy przepisów z algorytmem dopasowującym, a wykorzystaniem zewnętrznej sztucznej inteligencji.

## 2. Decyzja Architektoniczna (Decyzja)
Zdecydowano o wdrożeniu **architektury dwutorowej**. 
* Zaimplementowano sztywny algorytm klasyczny (baza lokalna) jako wariant zapasowy (failsafe).
* Zdecydowano o integracji z zewnętrznym modelem językowym **Google Gemini 3.5 Flash** poprzez REST API w celu generowania kreatywnych, niestatycznych przepisów w formacie JSON w czasie rzeczywistym. 

Do integracji użyto warstwy `GeminiService` na backendzie, separując logikę HTTP od kontrolera (zgodnie z Clean Architecture i CQRS).

## 3. Konsekwencje (Plusy i Minusy)
* **Plusy:** Nieskończona liczba wariantów posiłków dla użytkownika, odciążenie lokalnej bazy danych, wysoka elastyczność w dopasowywaniu gramatur do ułamków grama.
* **Minusy:** Uzależnienie od zewnętrznego dostawcy API (Google), ryzyko opóźnień sieciowych (latency) oraz konieczność bezpiecznego zarządzania kluczami (`GEMINI_API_KEY` w pliku `.env`).
* **Mitygacja ryzyk:** Wprowadzono na frontendzie mechanizm obronny `try-catch` (Fallback), który w razie awarii API serwuje predefiniowany, pełnowartościowy posiłek.

---

# Specyfikacja Funkcjonalności: Generator Posiłków "Chef AI"

## 1. Opis działania
Moduł realizuje zadanie automatycznego dobierania składników, gramatur oraz instrukcji przygotowania posiłków w oparciu o wprowadzone przez użytkownika docelowe wartości makroskładników: Białka (g), Węglowodanów (g) oraz Tłuszczów (g). 

Dzięki asynchronicznemu zapytaniu HTTP z użyciem precyzyjnego promptu inżynieryjnego, model AI analizuje makroskładniki i generuje kompletny przepis kulinarny. Zwraca dane w restrykcyjnym formacie JSON zawierającym: 
* Nazwę posiłku (`mealName`)
* Zważone składniki (`ingredients`) 
* Uporządkowane kroki przygotowania (`preparationSteps`).

## 2. Architektura i Wykaz Plików

Zastosowano zasady **Czystej Architektury (Clean Architecture)** oraz wzorzec **CQRS**.

**A. Warstwa Frontendu (React / Vite):**
* `src/Components/AiMealMatcher.tsx` – Odpowiada za UI. Zawiera formularz, walidację i renderuje dwa panele wyników. Implementuje mechanizm obronny (Fallback) chroniący przed wygasłym kluczem API czy brakiem internetu.
* `.env` (Frontend) – Przechowuje tajny token uwierzytelniający (nie trafia on do repozytorium kodu).

**B. Warstwa Backendowa (.NET Core):**
* `API/Controllers/AiMealController.cs` – "Chudy" kontroler. Mapuje parametry i przekazuje obiekt `GetGeminiMealQuery` do warstwy aplikacji przez szynę `MediatR`.
* `Domain/Services/GeminiService.cs` – Serce integracji AI. Odpowiada za:
  1. Zaczytanie klucza z pliku `.env`.
  2. Sanityzację klucza (wyrażenia regularne usuwające białe znaki zapobiegające błędom URI).
  3. Realizację asynchronicznego zapytania `POST` do Google API.
  4. Deserializację zwalidowanego formatu JSON na obiekty C#.

## 3. Schemat Przepływu Danych (Data Flow)
1. **Frontend:** Użytkownik wysyła formularz (metoda `handleGenerate`). Idzie żądanie GET (`/api/AiMeal/generate-ai`).
2. **Kontroler:** `AiMealController` odbiera zapytanie i wysyła je przez `IMediator`.
3. **Handler:** Szyna danych uruchamia `GetGeminiMealQueryHandler`, który przekazuje zadanie do `GeminiService`.
4. **Serwis (Autoryzacja):** `GeminiService` czyta i oczyszcza klucz z `.env`.
5. **Zapytanie do API:** Serwis wysyła `POST` do chmury Google Gemini z przygotowanym promptem.
6. **Odpowiedź API:** Serwis odbiera JSON, usuwa ewentualne formatowanie markdown i deserializuje obiekt.
7. **Prezentacja wyników:** Backend zwraca kod 200 OK, a interfejs graficzny renderuje listę składników i kroki przepisu.