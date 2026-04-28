---
name: test
description: Tworzy i uruchamia testy jednostkowe i integracyjne dla projektu
---

# Test - Testing Skill

Skill do tworzenia i uruchamiania testów w projekcie jemcochcem.pl.

## Kiedy używać

- Tworzenie testów dla nowej funkcjonalności
- Dodawanie testów do istniejącego kodu
- Uruchamianie testów przed commitem
- Weryfikacja pokrycia kodu testami
- Test-Driven Development (TDD)

## Typy testów

### 1. Unit Tests (Testy jednostkowe)
- Testują pojedyncze jednostki kodu (metody, funkcje)
- Izolowane od zależności (używają mocków)
- Szybkie wykonanie
- Wysokie pokrycie kodu

### 2. Integration Tests (Testy integracyjne)
- Testują współpracę komponentów
- Używają prawdziwych zależności
- Wolniejsze niż unit tests
- Testują flow end-to-end

### 3. Component Tests (Frontend)
- Testują komponenty React
- Sprawdzają rendering i interakcje
- Używają React Testing Library

## Backend Testing (C#/.NET)

### Setup - xUnit + NSubstitute

```csharp
// Backend.tests/Backend.tests.csproj
<ItemGroup>
  <PackageReference Include="xUnit" Version="2.4.2" />
  <PackageReference Include="xUnit.runner.visualstudio" Version="2.4.5" />
  <PackageReference Include="NSubstitute" Version="5.0.0" />
  <PackageReference Include="FluentAssertions" Version="6.11.0" />
  <PackageReference Include="Microsoft.EntityFrameworkCore.InMemory" Version="8.0.0" />
</ItemGroup>
```

### Unit Tests - Command Handler

```csharp
/// <summary>
/// [CO] Testy dla AddMealItemCommandHandler
/// [DLACZEGO] Weryfikacja logiki dodawania posiłku
/// [JAK] Używa NSubstitute do mockowania dependencies
/// </summary>
public class AddMealItemCommandHandlerTests
{
    private readonly IMealLogRepository _mockRepository;
    private readonly ILogger<AddMealItemCommandHandler> _mockLogger;
    private readonly AddMealItemCommandHandler _handler;

    public AddMealItemCommandHandlerTests()
    {
        _mockRepository = Substitute.For<IMealLogRepository>();
        _mockLogger = Substitute.For<ILogger<AddMealItemCommandHandler>>();
        _handler = new AddMealItemCommandHandler(_mockRepository, _mockLogger);
    }

    [Fact]
    public async Task Handle_ValidCommand_CreatesMealItem()
    {
        // Arrange
        var command = new AddMealItemCommand
        {
            UserId = Guid.NewGuid(),
            Date = DateTime.UtcNow,
            FoodName = "Apple",
            Calories = 95,
            Protein = 0.5m,
            Carbs = 25m,
            Fat = 0.3m
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBe(Guid.Empty);
        await _mockRepository.Received(1).AddAsync(Arg.Any<MealLog>());
        _mockLogger.Received().LogInformation(
            Arg.Is<string>(s => s.Contains("Meal item")),
            Arg.Any<object[]>()
        );
    }

    [Fact]
    public async Task Handle_InvalidUserId_ThrowsException()
    {
        // Arrange
        var command = new AddMealItemCommand
        {
            UserId = Guid.Empty, // Invalid
            Date = DateTime.UtcNow,
            FoodName = "Apple",
            Calories = 95
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => _handler.Handle(command, CancellationToken.None)
        );
    }

    [Theory]
    [InlineData("", 95)] // Empty name
    [InlineData("Apple", -10)] // Negative calories
    [InlineData("Apple", 0)] // Zero calories
    public async Task Handle_InvalidData_ThrowsException(string foodName, int calories)
    {
        // Arrange
        var command = new AddMealItemCommand
        {
            UserId = Guid.NewGuid(),
            Date = DateTime.UtcNow,
            FoodName = foodName,
            Calories = calories
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => _handler.Handle(command, CancellationToken.None)
        );
    }
}
```

### Unit Tests - Validator

```csharp
/// <summary>
/// Testy dla AddMealItemValidator
/// </summary>
public class AddMealItemValidatorTests
{
    private readonly AddMealItemValidator _validator;

    public AddMealItemValidatorTests()
    {
        _validator = new AddMealItemValidator();
    }

    [Fact]
    public void Validate_ValidCommand_ReturnsSuccess()
    {
        // Arrange
        var command = new AddMealItemCommand
        {
            UserId = Guid.NewGuid(),
            Date = DateTime.UtcNow,
            FoodName = "Apple",
            Calories = 95,
            Protein = 0.5m,
            Carbs = 25m,
            Fat = 0.3m
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_EmptyUserId_ReturnsError()
    {
        // Arrange
        var command = new AddMealItemCommand
        {
            UserId = Guid.Empty,
            Date = DateTime.UtcNow,
            FoodName = "Apple",
            Calories = 95
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "UserId");
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void Validate_InvalidFoodName_ReturnsError(string foodName)
    {
        // Arrange
        var command = new AddMealItemCommand
        {
            UserId = Guid.NewGuid(),
            Date = DateTime.UtcNow,
            FoodName = foodName,
            Calories = 95
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "FoodName");
    }
}
```

### Integration Tests - Repository

```csharp
/// <summary>
/// [CO] Testy integracyjne dla MealLogRepository
/// [DLACZEGO] Weryfikacja interakcji z bazą danych
/// [JAK] Używa InMemory database
/// </summary>
public class MealLogRepositoryIntegrationTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly MealLogRepository _repository;

    public MealLogRepositoryIntegrationTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _repository = new MealLogRepository(_context);
    }

    [Fact]
    public async Task AddAsync_ValidMealLog_SavesToDatabase()
    {
        // Arrange
        var mealLog = MealLog.Create(
            Guid.NewGuid(),
            DateTime.UtcNow,
            "Apple",
            95,
            0.5m,
            25m,
            0.3m
        );

        // Act
        await _repository.AddAsync(mealLog);

        // Assert
        var saved = await _context.MealLogs.FindAsync(mealLog.Id);
        saved.Should().NotBeNull();
        saved.FoodName.Should().Be("Apple");
    }

    [Fact]
    public async Task GetByUserIdAsync_ReturnsUserMealLogs()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var date = DateTime.UtcNow.Date;

        var mealLog1 = MealLog.Create(userId, date, "Apple", 95, 0.5m, 25m, 0.3m);
        var mealLog2 = MealLog.Create(userId, date, "Banana", 105, 1.3m, 27m, 0.4m);
        var mealLog3 = MealLog.Create(Guid.NewGuid(), date, "Orange", 62, 1.2m, 15m, 0.2m);

        await _repository.AddAsync(mealLog1);
        await _repository.AddAsync(mealLog2);
        await _repository.AddAsync(mealLog3);

        // Act
        var result = await _repository.GetByUserIdAsync(userId, date);

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(m => m.FoodName == "Apple");
        result.Should().Contain(m => m.FoodName == "Banana");
        result.Should().NotContain(m => m.FoodName == "Orange");
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
```

### Integration Tests - API Endpoint

```csharp
/// <summary>
/// Testy integracyjne dla DiaryController
/// </summary>
public class DiaryControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public DiaryControllerIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task AddMealItem_ValidRequest_ReturnsCreated()
    {
        // Arrange
        var request = new AddMealItemCommand
        {
            UserId = Guid.NewGuid(),
            Date = DateTime.UtcNow,
            FoodName = "Apple",
            Calories = 95,
            Protein = 0.5m,
            Carbs = 25m,
            Fat = 0.3m
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/diary", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var mealLogId = await response.Content.ReadFromJsonAsync<Guid>();
        mealLogId.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public async Task GetDailyDiary_ValidDate_ReturnsOk()
    {
        // Arrange
        var date = DateTime.UtcNow.Date;

        // Act
        var response = await _client.GetAsync($"/api/diary?date={date:yyyy-MM-dd}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var diary = await response.Content.ReadFromJsonAsync<DiaryDto>();
        diary.Should().NotBeNull();
    }
}
```

## Frontend Testing (React/TypeScript)

### Setup - Vitest + React Testing Library

```json
// package.json
{
  "devDependencies": {
    "vitest": "^1.0.0",
    "@testing-library/react": "^14.0.0",
    "@testing-library/jest-dom": "^6.1.0",
    "@testing-library/user-event": "^14.5.0",
    "@vitest/ui": "^1.0.0"
  },
  "scripts": {
    "test": "vitest",
    "test:ui": "vitest --ui",
    "test:coverage": "vitest --coverage"
  }
}
```

### Component Tests

```typescript
/**
 * [CO] Testy dla MealLogItem component
 * [DLACZEGO] Weryfikacja renderowania i interakcji
 * [JAK] Używa React Testing Library
 */

import { render, screen, fireEvent } from '@testing-library/react';
import { describe, it, expect, vi } from 'vitest';
import { MealLogItem } from './MealLogItem';

describe('MealLogItem', () => {
  const mockMealLog = {
    id: '1',
    foodName: 'Apple',
    calories: 95,
    protein: 0.5,
    carbs: 25,
    fat: 0.3,
  };

  it('renders meal log information', () => {
    render(<MealLogItem mealLog={mockMealLog} onDelete={() => {}} />);

    expect(screen.getByText('Apple')).toBeInTheDocument();
    expect(screen.getByText('95 kcal')).toBeInTheDocument();
    expect(screen.getByText(/0.5g protein/i)).toBeInTheDocument();
  });

  it('calls onDelete when delete button is clicked', () => {
    const onDelete = vi.fn();
    render(<MealLogItem mealLog={mockMealLog} onDelete={onDelete} />);

    const deleteButton = screen.getByRole('button', { name: /delete/i });
    fireEvent.click(deleteButton);

    expect(onDelete).toHaveBeenCalledWith('1');
    expect(onDelete).toHaveBeenCalledTimes(1);
  });

  it('displays macronutrients correctly', () => {
    render(<MealLogItem mealLog={mockMealLog} onDelete={() => {}} />);

    expect(screen.getByText(/0.5g protein/i)).toBeInTheDocument();
    expect(screen.getByText(/25g carbs/i)).toBeInTheDocument();
    expect(screen.getByText(/0.3g fat/i)).toBeInTheDocument();
  });
});
```

### Hook Tests

```typescript
/**
 * Testy dla useDiary hook
 */

import { renderHook, waitFor } from '@testing-library/react';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { describe, it, expect, vi, beforeEach } from 'vitest';
import { useDiary } from './useDiary';
import { diaryApi } from '../api/diary-api';

// Mock API
vi.mock('../api/diary-api');

describe('useDiary', () => {
  let queryClient: QueryClient;

  beforeEach(() => {
    queryClient = new QueryClient({
      defaultOptions: {
        queries: { retry: false },
      },
    });
  });

  const wrapper = ({ children }: { children: React.ReactNode }) => (
    <QueryClientProvider client={queryClient}>
      {children}
    </QueryClientProvider>
  );

  it('fetches diary data successfully', async () => {
    const mockData = [
      { id: '1', foodName: 'Apple', calories: 95 },
      { id: '2', foodName: 'Banana', calories: 105 },
    ];

    vi.mocked(diaryApi.getDaily).mockResolvedValue(mockData);

    const { result } = renderHook(() => useDiary(new Date()), { wrapper });

    await waitFor(() => expect(result.current.isLoading).toBe(false));

    expect(result.current.mealLogs).toEqual(mockData);
    expect(result.current.error).toBeNull();
  });

  it('handles error when fetching fails', async () => {
    const error = new Error('Network error');
    vi.mocked(diaryApi.getDaily).mockRejectedValue(error);

    const { result } = renderHook(() => useDiary(new Date()), { wrapper });

    await waitFor(() => expect(result.current.isLoading).toBe(false));

    expect(result.current.error).toBeTruthy();
    expect(result.current.mealLogs).toBeUndefined();
  });
});
```

### Integration Tests - User Flow

```typescript
/**
 * Test integracyjny - dodawanie posiłku
 */

import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { describe, it, expect, vi } from 'vitest';
import { DiaryPage } from './DiaryPage';
import { diaryApi } from '../api/diary-api';

vi.mock('../api/diary-api');

describe('DiaryPage - Add Meal Flow', () => {
  it('allows user to add a new meal', async () => {
    const user = userEvent.setup();
    
    vi.mocked(diaryApi.getDaily).mockResolvedValue([]);
    vi.mocked(diaryApi.addMealItem).mockResolvedValue('new-id');

    render(<DiaryPage />);

    // Wypełnij formularz
    const foodNameInput = screen.getByLabelText(/food name/i);
    const caloriesInput = screen.getByLabelText(/calories/i);
    const submitButton = screen.getByRole('button', { name: /add meal/i });

    await user.type(foodNameInput, 'Apple');
    await user.type(caloriesInput, '95');
    await user.click(submitButton);

    // Sprawdź czy API zostało wywołane
    await waitFor(() => {
      expect(diaryApi.addMealItem).toHaveBeenCalledWith({
        foodName: 'Apple',
        calories: 95,
        date: expect.any(String),
      });
    });

    // Sprawdź czy formularz został wyczyszczony
    expect(foodNameInput).toHaveValue('');
    expect(caloriesInput).toHaveValue(null);
  });
});
```

## Test-Driven Development (TDD)

### Red-Green-Refactor Cycle

```
1. RED - Napisz test który failuje
2. GREEN - Napisz minimalny kod żeby test przeszedł
3. REFACTOR - Popraw kod zachowując testy zielone
```

**Przykład TDD:**

```csharp
// 1. RED - Test który failuje
[Fact]
public void CalculateTdee_ValidUser_ReturnsCorrectValue()
{
    // Arrange
    var user = new User
    {
        Weight = 70,
        Height = 175,
        Age = 30,
        Gender = Gender.Male,
        ActivityLevel = ActivityLevel.Moderate
    };
    var calculator = new TdeeCalculator();

    // Act
    var tdee = calculator.Calculate(user);

    // Assert
    tdee.Should().BeApproximately(2500, 50); // ±50 kcal
}

// 2. GREEN - Minimalna implementacja
public class TdeeCalculator
{
    public decimal Calculate(User user)
    {
        // Mifflin-St Jeor equation
        var bmr = (10 * user.Weight) + (6.25 * user.Height) - (5 * user.Age) + 5;
        return bmr * GetActivityMultiplier(user.ActivityLevel);
    }

    private decimal GetActivityMultiplier(ActivityLevel level)
    {
        return level switch
        {
            ActivityLevel.Sedentary => 1.2m,
            ActivityLevel.Light => 1.375m,
            ActivityLevel.Moderate => 1.55m,
            ActivityLevel.Active => 1.725m,
            ActivityLevel.VeryActive => 1.9m,
            _ => 1.2m
        };
    }
}

// 3. REFACTOR - Popraw kod (testy nadal zielone)
```

## Uruchamianie testów

### Backend
```bash
# Wszystkie testy
dotnet test

# Z coverage
dotnet test --collect:"XPlat Code Coverage"

# Konkretny test
dotnet test --filter "FullyQualifiedName~AddMealItemCommandHandlerTests"

# Verbose output
dotnet test --logger "console;verbosity=detailed"
```

### Frontend
```bash
# Wszystkie testy
npm test

# Watch mode
npm test -- --watch

# Coverage
npm run test:coverage

# UI mode
npm run test:ui

# Konkretny plik
npm test -- MealLogItem.test.tsx
```

## Checklist testowania

### Przed napisaniem testów
- [ ] Zrozum wymagania funkcjonalności
- [ ] Zidentyfikuj edge cases
- [ ] Określ co powinno być testowane
- [ ] Przygotuj test data

### Podczas pisania testów
- [ ] Używaj AAA pattern (Arrange-Act-Assert)
- [ ] Jeden test = jedna asercja (w miarę możliwości)
- [ ] Jasne nazwy testów (opisują co testują)
- [ ] Mockuj zależności zewnętrzne
- [ ] Testuj edge cases i error scenarios

### Po napisaniu testów
- [ ] Wszystkie testy przechodzą
- [ ] Coverage >70%
- [ ] Testy są szybkie (<1s per test)
- [ ] Brak flaky tests
- [ ] Dokumentacja testów

## Best Practices

### Nazewnictwo testów
```csharp
// Pattern: MethodName_Scenario_ExpectedBehavior
[Fact]
public void Calculate_ValidUser_ReturnsPositiveValue() { }

[Fact]
public void Calculate_NullUser_ThrowsArgumentNullException() { }

[Fact]
public void Calculate_NegativeWeight_ThrowsArgumentException() { }
```

### AAA Pattern
```csharp
[Fact]
public void Example_Test()
{
    // Arrange - przygotuj dane i dependencies
    var repository = Substitute.For<IRepository>();
    var service = new Service(repository);
    var input = new Input { Value = 10 };

    // Act - wykonaj testowaną operację
    var result = service.Process(input);

    // Assert - sprawdź wynik
    result.Should().Be(20);
}
```

### Test Data Builders
```csharp
public class UserBuilder
{
    private string _name = "John Doe";
    private string _email = "john@example.com";
    private int _age = 30;

    public UserBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public UserBuilder WithEmail(string email)
    {
        _email = email;
        return this;
    }

    public User Build()
    {
        return new User
        {
            Name = _name,
            Email = _email,
            Age = _age
        };
    }
}

// Użycie
var user = new UserBuilder()
    .WithName("Jane")
    .WithEmail("jane@example.com")
    .Build();
```

## Przykład użycia

```
User: "Napisz testy dla AddMealItemCommandHandler"

Cline (używając skill test):
1. Analizuje handler i jego dependencies
2. Tworzy test class z setup
3. Pisze testy:
   - Happy path (valid command)
   - Edge cases (empty values, nulls)
   - Error scenarios (exceptions)
4. Uruchamia testy: dotnet test
5. Sprawdza coverage
6. Raportuje wyniki
```
