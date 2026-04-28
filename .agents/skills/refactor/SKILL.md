---
name: refactor
description: Przeprowadza refaktoryzację kodu zgodnie z zasadami Clean Code, SOLID i Clean Architecture
---

# Refactor - Code Refactoring Skill

Skill do przeprowadzania profesjonalnej refaktoryzacji kodu w projekcie jemcochcem.pl.

## Kiedy używać

- Kod działa, ale jest nieczytelny lub trudny w utrzymaniu
- Duplikacja kodu (DRY violation)
- Długie funkcje/klasy (>50 linii)
- Naruszenie SOLID principles
- Technical debt cleanup
- Przed dodaniem nowych features
- Po code review z uwagami

## Proces Refaktoryzacji

### 1. Analiza i Planowanie
- [ ] Przeczytaj `.clinerules/rules.md` - standardy projektu
- [ ] Sprawdź `UML/plantUML-kod.txt` - architektura
- [ ] Zidentyfikuj code smells
- [ ] Określ scope refaktoryzacji
- [ ] Upewnij się że są testy (jeśli nie - napisz najpierw!)

### 2. Code Smells - Co refaktoryzować?

#### 🔴 Critical (Natychmiast)
- [ ] **God Class** - klasa robi za dużo (>500 linii)
- [ ] **Long Method** - funkcja >50 linii
- [ ] **Duplicate Code** - ten sam kod w wielu miejscach
- [ ] **Magic Numbers** - hardcoded wartości bez nazw
- [ ] **Dead Code** - nieużywany kod
- [ ] **Commented Code** - zakomentowany kod (usuń!)

#### 🟡 Important (Wkrótce)
- [ ] **Feature Envy** - metoda używa więcej z innej klasy
- [ ] **Data Clumps** - te same grupy parametrów
- [ ] **Primitive Obsession** - używanie primitives zamiast Value Objects
- [ ] **Switch Statements** - długie switch/if-else (użyj Strategy Pattern)
- [ ] **Lazy Class** - klasa robi za mało
- [ ] **Speculative Generality** - kod "na przyszłość"

#### 🟢 Nice to Have (Gdy jest czas)
- [ ] **Middle Man** - klasa tylko deleguje
- [ ] **Inappropriate Intimacy** - klasy za bardzo ze sobą związane
- [ ] **Message Chains** - długie łańcuchy wywołań
- [ ] **Incomplete Library Class** - brakujące metody utility

### 3. Refactoring Patterns

#### Extract Method
```csharp
// ❌ PRZED
public void ProcessOrder(Order order)
{
    // Walidacja
    if (order == null) throw new ArgumentNullException();
    if (order.Items.Count == 0) throw new InvalidOperationException();
    
    // Obliczenia
    decimal total = 0;
    foreach (var item in order.Items)
    {
        total += item.Price * item.Quantity;
    }
    
    // Zapis
    _db.Orders.Add(order);
    _db.SaveChanges();
}

// ✅ PO
public void ProcessOrder(Order order)
{
    ValidateOrder(order);
    var total = CalculateTotal(order);
    SaveOrder(order);
}

private void ValidateOrder(Order order)
{
    if (order == null) throw new ArgumentNullException(nameof(order));
    if (order.Items.Count == 0) throw new InvalidOperationException("Order must have items");
}

private decimal CalculateTotal(Order order)
{
    return order.Items.Sum(item => item.Price * item.Quantity);
}

private void SaveOrder(Order order)
{
    _db.Orders.Add(order);
    _db.SaveChanges();
}
```

#### Extract Class
```csharp
// ❌ PRZED - God Class
public class User
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string Street { get; set; }
    public string City { get; set; }
    public string ZipCode { get; set; }
    public string Country { get; set; }
}

// ✅ PO - Separation of Concerns
public class User
{
    public string Name { get; set; }
    public string Email { get; set; }
    public Address Address { get; set; }
}

public class Address
{
    public string Street { get; set; }
    public string City { get; set; }
    public string ZipCode { get; set; }
    public string Country { get; set; }
}
```

#### Replace Magic Numbers
```csharp
// ❌ PRZED
if (user.Age >= 18 && user.Age <= 65)
{
    discount = price * 0.1m;
}

// ✅ PO
private const int MinAdultAge = 18;
private const int MaxWorkingAge = 65;
private const decimal StandardDiscount = 0.1m;

if (user.Age >= MinAdultAge && user.Age <= MaxWorkingAge)
{
    discount = price * StandardDiscount;
}
```

#### Replace Conditional with Polymorphism
```csharp
// ❌ PRZED
public decimal CalculatePrice(string customerType, decimal basePrice)
{
    switch (customerType)
    {
        case "Regular":
            return basePrice;
        case "Premium":
            return basePrice * 0.9m;
        case "VIP":
            return basePrice * 0.8m;
        default:
            throw new ArgumentException("Unknown customer type");
    }
}

// ✅ PO - Strategy Pattern
public interface ICustomerPricingStrategy
{
    decimal CalculatePrice(decimal basePrice);
}

public class RegularCustomerPricing : ICustomerPricingStrategy
{
    public decimal CalculatePrice(decimal basePrice) => basePrice;
}

public class PremiumCustomerPricing : ICustomerPricingStrategy
{
    public decimal CalculatePrice(decimal basePrice) => basePrice * 0.9m;
}

public class VipCustomerPricing : ICustomerPricingStrategy
{
    public decimal CalculatePrice(decimal basePrice) => basePrice * 0.8m;
}
```

### 4. Clean Architecture Refactoring

#### Backend - Warstwa Domain
```csharp
// ✅ Domain powinien zawierać:
// - Entities (User, MealLog, Product)
// - Value Objects (MacroNutrients, Email, Weight)
// - Domain Events
// - Interfaces (IUserRepository, IMealLogRepository)
// - Domain Exceptions

// ❌ Domain NIE POWINIEN zawierać:
// - EF Core dependencies
// - HTTP dependencies
// - External service clients
// - Infrastructure code
```

#### Backend - Warstwa Application
```csharp
// ✅ Application powinien zawierać:
// - Commands (AddMealItemCommand)
// - Queries (GetDailyDiaryQuery)
// - Handlers (MediatR)
// - Validators (FluentValidation)
// - DTOs dla response

// ❌ Application NIE POWINIEN zawierać:
// - Database context
// - HTTP controllers
// - External API clients (tylko interfejsy)
```

#### Frontend - Component Refactoring
```typescript
// ❌ PRZED - Wszystko w jednym komponencie
export function DiaryPage() {
  const [meals, setMeals] = useState([]);
  const [loading, setLoading] = useState(false);
  
  useEffect(() => {
    setLoading(true);
    fetch('/api/diary')
      .then(res => res.json())
      .then(data => setMeals(data))
      .finally(() => setLoading(false));
  }, []);
  
  return (
    <div>
      {loading ? <div>Loading...</div> : null}
      {meals.map(meal => (
        <div key={meal.id}>
          <h3>{meal.name}</h3>
          <p>{meal.calories} kcal</p>
        </div>
      ))}
    </div>
  );
}

// ✅ PO - Separation of Concerns
// hooks/useDiary.ts
export function useDiary(date: Date) {
  return useQuery({
    queryKey: ['diary', date],
    queryFn: () => diaryApi.getDaily(date),
  });
}

// components/MealItem.tsx
export function MealItem({ meal }: { meal: Meal }) {
  return (
    <div className="meal-item">
      <h3>{meal.name}</h3>
      <p>{meal.calories} kcal</p>
    </div>
  );
}

// pages/DiaryPage.tsx
export function DiaryPage() {
  const { data: meals, isLoading } = useDiary(new Date());
  
  if (isLoading) return <LoadingSpinner />;
  
  return (
    <div>
      {meals?.map(meal => (
        <MealItem key={meal.id} meal={meal} />
      ))}
    </div>
  );
}
```

### 5. Checklist Refaktoryzacji

#### Przed rozpoczęciem
- [ ] Upewnij się że są testy (coverage >70%)
- [ ] Zrób commit obecnego stanu
- [ ] Utwórz branch `refactor/nazwa-refaktoryzacji`
- [ ] Zidentyfikuj wszystkie miejsca do zmiany

#### Podczas refaktoryzacji
- [ ] Refaktoryzuj małymi krokami
- [ ] Po każdym kroku uruchom testy
- [ ] Commituj często (każda zmiana = commit)
- [ ] Nie zmieniaj funkcjonalności (tylko strukturę)
- [ ] Dodaj/zaktualizuj komentarze

#### Po refaktoryzacji
- [ ] Wszystkie testy przechodzą
- [ ] Kod jest bardziej czytelny
- [ ] Brak duplikacji
- [ ] SOLID principles przestrzegane
- [ ] Performance nie pogorszył się
- [ ] Dokumentacja zaktualizowana

### 6. SOLID Principles - Praktyczne zastosowanie

#### Single Responsibility Principle
```csharp
// ❌ PRZED - klasa robi za dużo
public class UserService
{
    public void RegisterUser(UserDto dto) { }
    public void SendEmail(string email) { }
    public void LogActivity(string message) { }
    public void ValidateUser(User user) { }
}

// ✅ PO - każda klasa ma jedną odpowiedzialność
public class UserService
{
    private readonly IEmailService _emailService;
    private readonly ILogger _logger;
    private readonly IUserValidator _validator;
    
    public void RegisterUser(UserDto dto)
    {
        var user = MapToUser(dto);
        _validator.Validate(user);
        SaveUser(user);
        _emailService.SendWelcomeEmail(user.Email);
        _logger.LogInfo($"User {user.Id} registered");
    }
}
```

#### Open/Closed Principle
```csharp
// ✅ Otwarty na rozszerzenia, zamknięty na modyfikacje
public interface INotificationSender
{
    Task SendAsync(string message, string recipient);
}

public class EmailNotificationSender : INotificationSender { }
public class SmsNotificationSender : INotificationSender { }
public class PushNotificationSender : INotificationSender { }

// Dodanie nowego typu nie wymaga modyfikacji istniejącego kodu
```

#### Dependency Inversion Principle
```csharp
// ❌ PRZED - zależność od konkretnej implementacji
public class OrderService
{
    private readonly SqlServerRepository _repository = new SqlServerRepository();
}

// ✅ PO - zależność od abstrakcji
public class OrderService
{
    private readonly IOrderRepository _repository;
    
    public OrderService(IOrderRepository repository)
    {
        _repository = repository;
    }
}
```

### 7. Performance Refactoring

#### Backend - Database Queries
```csharp
// ❌ PRZED - N+1 problem
var users = await _context.Users.ToListAsync();
foreach (var user in users)
{
    user.Orders = await _context.Orders
        .Where(o => o.UserId == user.Id)
        .ToListAsync();
}

// ✅ PO - Eager loading
var users = await _context.Users
    .Include(u => u.Orders)
    .ToListAsync();
```

#### Frontend - React Performance
```typescript
// ❌ PRZED - niepotrzebne re-renders
function ExpensiveComponent({ data, onUpdate }) {
  const processedData = expensiveCalculation(data);
  return <div>{processedData}</div>;
}

// ✅ PO - memoization
function ExpensiveComponent({ data, onUpdate }) {
  const processedData = useMemo(
    () => expensiveCalculation(data),
    [data]
  );
  
  const handleUpdate = useCallback(
    (value) => onUpdate(value),
    [onUpdate]
  );
  
  return <div>{processedData}</div>;
}
```

### 8. Narzędzia pomocnicze

#### Backend
```bash
# Analiza kodu
dotnet build /p:TreatWarningsAsErrors=true

# Code metrics
dotnet tool install -g dotnet-code-metrics

# Testy
dotnet test --collect:"XPlat Code Coverage"
```

#### Frontend
```bash
# Type checking
npm run type-check

# Linting
npm run lint

# Bundle analysis
npm run build -- --analyze
```

## Output Refaktoryzacji

Po zakończeniu dostarcz:

### 1. Podsumowanie zmian
```
📊 Refactoring Summary

Pliki zmienione: 12
Linie dodane: +234
Linie usunięte: -456
Code smells usunięte: 8

Główne zmiany:
- Extracted 5 methods from UserService
- Created Address Value Object
- Replaced switch with Strategy Pattern
- Removed 3 God Classes
```

### 2. Metryki
```
Przed:
- Średnia długość metody: 45 linii
- Cyclomatic complexity: 12
- Code coverage: 65%

Po:
- Średnia długość metody: 18 linii
- Cyclomatic complexity: 4
- Code coverage: 78%
```

### 3. Rekomendacje dalsze
- Lista pozostałych code smells
- Propozycje kolejnych refaktoryzacji
- Technical debt do spłacenia

## Zasady bezpiecznej refaktoryzacji

1. **Red-Green-Refactor** (TDD cycle)
   - Red: Napisz test (fail)
   - Green: Napisz kod (pass)
   - Refactor: Popraw kod (still pass)

2. **Małe kroki**
   - Jedna zmiana na raz
   - Commit po każdej zmianie
   - Testy po każdym kroku

3. **Nie zmieniaj funkcjonalności**
   - Refactoring = zmiana struktury, nie behavior
   - Jeśli zmieniasz behavior, to nie refactoring

4. **Zachowaj testy zielone**
   - Wszystkie testy muszą przechodzić
   - Jeśli test fail, cofnij zmianę

## Przykład użycia

```
User: "Zrefaktoryzuj UserService - jest za długi i robi za dużo"

Cline (używając skill refactor):
1. Analizuje UserService
2. Identyfikuje code smells (God Class, Long Methods)
3. Planuje refaktoryzację (Extract Class, Extract Method)
4. Wykonuje zmiany małymi krokami
5. Uruchamia testy po każdej zmianie
6. Dostarcza raport z metrykami
```
