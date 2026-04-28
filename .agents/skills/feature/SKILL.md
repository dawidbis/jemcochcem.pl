---
name: feature
description: Implementuje nową funkcjonalność zgodnie z Clean Architecture i standardami projektu
---

# Feature - New Feature Implementation Skill

Skill do implementacji nowych funkcjonalności w projekcie jemcochcem.pl zgodnie z Clean Architecture.

## Kiedy używać

- Implementacja nowej funkcjonalności
- Dodanie nowego endpointu API
- Utworzenie nowego komponentu UI
- Rozszerzenie istniejącej funkcjonalności
- Integracja z zewnętrznym API

## Proces Implementacji

### 1. Analiza i Planowanie
- [ ] Przeczytaj `README.md` - sprawdź aktualną iterację (MVP/Analytics/AI)
- [ ] Przeczytaj `.clinerules/rules.md` - standardy projektu
- [ ] Sprawdź `UML/plantUML-kod.txt` - architektura i flow danych
- [ ] Zidentyfikuj warstwy które będą dotknięte
- [ ] Sprawdź czy podobna funkcjonalność już istnieje (Ctrl+Shift+F)
- [ ] Zaplanuj strukturę plików i klas

### 2. Workflow Backend (C#/.NET)

#### Krok 1: Domain Layer (jeśli potrzebne)
```
Backend/FitApp.Domain/
├── Entities/           # Nowe entity
├── ValueObjects/       # Nowe value objects
├── Interfaces/         # Nowe repository interfaces
└── Exceptions/         # Domain-specific exceptions
```

**Przykład - Nowy Entity:**
```csharp
/// <summary>
/// [CO] Reprezentuje wpis w dzienniku treningowym użytkownika
/// [DLACZEGO] Potrzebujemy śledzić aktywność fizyczną dla kalkulacji TDEE
/// [JAK] Przechowuje typ ćwiczenia, czas trwania i spalone kalorie
/// </summary>
public class WorkoutLog
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public DateTime Date { get; private set; }
    public string ExerciseType { get; private set; }
    public int DurationMinutes { get; private set; }
    public int CaloriesBurned { get; private set; }
    
    // Private constructor - tylko przez factory method
    private WorkoutLog() { }
    
    /// <summary>
    /// Factory method do tworzenia nowego workout log
    /// </summary>
    public static WorkoutLog Create(
        Guid userId, 
        DateTime date, 
        string exerciseType, 
        int durationMinutes, 
        int caloriesBurned)
    {
        // Walidacja domenowa
        if (userId == Guid.Empty)
            throw new ArgumentException("UserId cannot be empty");
        if (durationMinutes <= 0)
            throw new ArgumentException("Duration must be positive");
            
        return new WorkoutLog
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Date = date,
            ExerciseType = exerciseType,
            DurationMinutes = durationMinutes,
            CaloriesBurned = caloriesBurned
        };
    }
}
```

**Przykład - Repository Interface:**
```csharp
/// <summary>
/// Interface dla dostępu do danych workout logs
/// </summary>
public interface IWorkoutLogRepository
{
    Task<WorkoutLog?> GetByIdAsync(Guid id);
    Task<IEnumerable<WorkoutLog>> GetByUserIdAsync(Guid userId, DateTime date);
    Task AddAsync(WorkoutLog workoutLog);
    Task UpdateAsync(WorkoutLog workoutLog);
    Task DeleteAsync(Guid id);
}
```

#### Krok 2: Application Layer (CQRS)
```
Backend/FitApp.Application/Features/Workout/
├── Commands/
│   ├── AddWorkoutLogCommand.cs
│   ├── AddWorkoutLogCommandHandler.cs
│   └── AddWorkoutLogValidator.cs
└── Queries/
    ├── GetWorkoutLogsQuery.cs
    └── GetWorkoutLogsQueryHandler.cs
```

**Przykład - Command:**
```csharp
/// <summary>
/// [CO] Command do dodania nowego workout log
/// [DLACZEGO] Użytkownik chce zalogować swoją aktywność fizyczną
/// </summary>
public class AddWorkoutLogCommand : IRequest<Guid>
{
    public Guid UserId { get; set; }
    public DateTime Date { get; set; }
    public string ExerciseType { get; set; }
    public int DurationMinutes { get; set; }
    public int CaloriesBurned { get; set; }
}

/// <summary>
/// Handler dla AddWorkoutLogCommand
/// </summary>
public class AddWorkoutLogCommandHandler : IRequestHandler<AddWorkoutLogCommand, Guid>
{
    private readonly IWorkoutLogRepository _repository;
    private readonly ILogger<AddWorkoutLogCommandHandler> _logger;
    
    public AddWorkoutLogCommandHandler(
        IWorkoutLogRepository repository,
        ILogger<AddWorkoutLogCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }
    
    public async Task<Guid> Handle(AddWorkoutLogCommand request, CancellationToken cancellationToken)
    {
        // Tworzenie domain entity
        var workoutLog = WorkoutLog.Create(
            request.UserId,
            request.Date,
            request.ExerciseType,
            request.DurationMinutes,
            request.CaloriesBurned
        );
        
        // Zapis do bazy
        await _repository.AddAsync(workoutLog);
        
        _logger.LogInformation(
            "Workout log {WorkoutLogId} created for user {UserId}", 
            workoutLog.Id, 
            request.UserId
        );
        
        return workoutLog.Id;
    }
}
```

**Przykład - Validator:**
```csharp
/// <summary>
/// Walidator dla AddWorkoutLogCommand
/// </summary>
public class AddWorkoutLogValidator : AbstractValidator<AddWorkoutLogCommand>
{
    public AddWorkoutLogValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("UserId is required");
            
        RuleFor(x => x.Date)
            .NotEmpty()
            .LessThanOrEqualTo(DateTime.UtcNow)
            .WithMessage("Date cannot be in the future");
            
        RuleFor(x => x.ExerciseType)
            .NotEmpty()
            .MaximumLength(100)
            .WithMessage("Exercise type must be between 1 and 100 characters");
            
        RuleFor(x => x.DurationMinutes)
            .GreaterThan(0)
            .LessThanOrEqualTo(1440) // 24 hours
            .WithMessage("Duration must be between 1 and 1440 minutes");
            
        RuleFor(x => x.CaloriesBurned)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Calories burned cannot be negative");
    }
}
```

**Przykład - Query:**
```csharp
/// <summary>
/// Query do pobrania workout logs dla użytkownika
/// </summary>
public class GetWorkoutLogsQuery : IRequest<IEnumerable<WorkoutLogDto>>
{
    public Guid UserId { get; set; }
    public DateTime Date { get; set; }
}

/// <summary>
/// Handler dla GetWorkoutLogsQuery
/// </summary>
public class GetWorkoutLogsQueryHandler : IRequestHandler<GetWorkoutLogsQuery, IEnumerable<WorkoutLogDto>>
{
    private readonly IWorkoutLogRepository _repository;
    
    public GetWorkoutLogsQueryHandler(IWorkoutLogRepository repository)
    {
        _repository = repository;
    }
    
    public async Task<IEnumerable<WorkoutLogDto>> Handle(
        GetWorkoutLogsQuery request, 
        CancellationToken cancellationToken)
    {
        var workoutLogs = await _repository.GetByUserIdAsync(request.UserId, request.Date);
        
        return workoutLogs.Select(w => new WorkoutLogDto
        {
            Id = w.Id,
            Date = w.Date,
            ExerciseType = w.ExerciseType,
            DurationMinutes = w.DurationMinutes,
            CaloriesBurned = w.CaloriesBurned
        });
    }
}
```

#### Krok 3: Infrastructure Layer
```
Backend/FitApp.Infrastructure/
├── Data/Repositories/
│   └── WorkoutLogRepository.cs
└── Data/Configurations/
    └── WorkoutLogConfiguration.cs
```

**Przykład - Repository Implementation:**
```csharp
/// <summary>
/// Implementacja IWorkoutLogRepository używając EF Core
/// </summary>
public class WorkoutLogRepository : IWorkoutLogRepository
{
    private readonly ApplicationDbContext _context;
    
    public WorkoutLogRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<WorkoutLog?> GetByIdAsync(Guid id)
    {
        return await _context.WorkoutLogs
            .FirstOrDefaultAsync(w => w.Id == id);
    }
    
    public async Task<IEnumerable<WorkoutLog>> GetByUserIdAsync(Guid userId, DateTime date)
    {
        return await _context.WorkoutLogs
            .Where(w => w.UserId == userId && w.Date.Date == date.Date)
            .OrderBy(w => w.Date)
            .ToListAsync();
    }
    
    public async Task AddAsync(WorkoutLog workoutLog)
    {
        await _context.WorkoutLogs.AddAsync(workoutLog);
        await _context.SaveChangesAsync();
    }
    
    public async Task UpdateAsync(WorkoutLog workoutLog)
    {
        _context.WorkoutLogs.Update(workoutLog);
        await _context.SaveChangesAsync();
    }
    
    public async Task DeleteAsync(Guid id)
    {
        var workoutLog = await GetByIdAsync(id);
        if (workoutLog != null)
        {
            _context.WorkoutLogs.Remove(workoutLog);
            await _context.SaveChangesAsync();
        }
    }
}
```

**Przykład - EF Configuration:**
```csharp
/// <summary>
/// Konfiguracja EF Core dla WorkoutLog entity
/// </summary>
public class WorkoutLogConfiguration : IEntityTypeConfiguration<WorkoutLog>
{
    public void Configure(EntityTypeBuilder<WorkoutLog> builder)
    {
        builder.HasKey(w => w.Id);
        
        builder.Property(w => w.ExerciseType)
            .IsRequired()
            .HasMaxLength(100);
            
        builder.Property(w => w.DurationMinutes)
            .IsRequired();
            
        builder.Property(w => w.CaloriesBurned)
            .IsRequired();
            
        builder.HasIndex(w => new { w.UserId, w.Date });
    }
}
```

#### Krok 4: API Layer (Controller)
```
Backend/FitApp.API/Controllers/
└── WorkoutController.cs
```

**Przykład - Controller:**
```csharp
/// <summary>
/// Controller do zarządzania workout logs
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WorkoutController : ControllerBase
{
    private readonly IMediator _mediator;
    
    public WorkoutController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    /// <summary>
    /// Dodaje nowy workout log
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddWorkoutLog([FromBody] AddWorkoutLogCommand command)
    {
        var workoutLogId = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetWorkoutLogs), new { date = command.Date }, workoutLogId);
    }
    
    /// <summary>
    /// Pobiera workout logs dla użytkownika
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<WorkoutLogDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetWorkoutLogs([FromQuery] DateTime date)
    {
        var userId = User.GetUserId(); // Extension method
        var query = new GetWorkoutLogsQuery { UserId = userId, Date = date };
        var workoutLogs = await _mediator.Send(query);
        return Ok(workoutLogs);
    }
}
```

#### Krok 5: Dependency Injection (Program.cs)
```csharp
// Rejestracja w Program.cs
builder.Services.AddScoped<IWorkoutLogRepository, WorkoutLogRepository>();
```

### 3. Workflow Frontend (React/TypeScript)

#### Krok 1: Types
```
frontend/src/features/workout/types/
└── WorkoutTypes.ts
```

**Przykład - Types:**
```typescript
/**
 * [CO] Typy dla workout feature
 * [DLACZEGO] TypeScript strict mode wymaga proper typing
 */

export interface WorkoutLog {
  id: string;
  date: string;
  exerciseType: string;
  durationMinutes: number;
  caloriesBurned: number;
}

export interface AddWorkoutLogRequest {
  date: string;
  exerciseType: string;
  durationMinutes: number;
  caloriesBurned: number;
}

export interface WorkoutLogsResponse {
  workoutLogs: WorkoutLog[];
  totalCaloriesBurned: number;
}
```

#### Krok 2: API Client
```
frontend/src/features/workout/api/
└── workout-api.ts
```

**Przykład - API Client:**
```typescript
/**
 * [CO] API client dla workout endpoints
 * [DLACZEGO] Centralizacja wywołań API
 * [JAK] Używa axios z proper error handling
 */

import { apiClient } from '@/lib/api-client';
import type { WorkoutLog, AddWorkoutLogRequest } from '../types/WorkoutTypes';

export const workoutApi = {
  /**
   * Pobiera workout logs dla danej daty
   */
  getWorkoutLogs: async (date: Date): Promise<WorkoutLog[]> => {
    const response = await apiClient.get<WorkoutLog[]>('/api/workout', {
      params: { date: date.toISOString() },
    });
    return response.data;
  },

  /**
   * Dodaje nowy workout log
   */
  addWorkoutLog: async (request: AddWorkoutLogRequest): Promise<string> => {
    const response = await apiClient.post<string>('/api/workout', request);
    return response.data;
  },

  /**
   * Usuwa workout log
   */
  deleteWorkoutLog: async (id: string): Promise<void> => {
    await apiClient.delete(`/api/workout/${id}`);
  },
};
```

#### Krok 3: Custom Hook
```
frontend/src/features/workout/hooks/
└── useWorkout.ts
```

**Przykład - Custom Hook:**
```typescript
/**
 * [CO] Hook do zarządzania workout logs
 * [DLACZEGO] Enkapsulacja logiki i state management
 * [JAK] Używa TanStack Query dla server state
 */

import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { workoutApi } from '../api/workout-api';
import type { AddWorkoutLogRequest } from '../types/WorkoutTypes';

export function useWorkout(date: Date) {
  const queryClient = useQueryClient();

  // Query - pobieranie workout logs
  const { data: workoutLogs, isLoading, error } = useQuery({
    queryKey: ['workout', date],
    queryFn: () => workoutApi.getWorkoutLogs(date),
    staleTime: 30000, // 30 sekund
  });

  // Mutation - dodawanie workout log
  const addWorkoutMutation = useMutation({
    mutationFn: (request: AddWorkoutLogRequest) => 
      workoutApi.addWorkoutLog(request),
    onSuccess: () => {
      // Invalidate i refetch
      queryClient.invalidateQueries({ queryKey: ['workout', date] });
    },
  });

  // Mutation - usuwanie workout log
  const deleteWorkoutMutation = useMutation({
    mutationFn: (id: string) => workoutApi.deleteWorkoutLog(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['workout', date] });
    },
  });

  return {
    workoutLogs,
    isLoading,
    error,
    addWorkout: addWorkoutMutation.mutate,
    deleteWorkout: deleteWorkoutMutation.mutate,
    isAdding: addWorkoutMutation.isPending,
    isDeleting: deleteWorkoutMutation.isPending,
  };
}
```

#### Krok 4: Components
```
frontend/src/features/workout/components/
├── WorkoutLogItem.tsx
├── WorkoutLogList.tsx
└── AddWorkoutForm.tsx
```

**Przykład - Component:**
```typescript
/**
 * [CO] Komponent do wyświetlania pojedynczego workout log
 * [DLACZEGO] Reusable component dla listy
 */

import type { WorkoutLog } from '../types/WorkoutTypes';

interface WorkoutLogItemProps {
  workoutLog: WorkoutLog;
  onDelete: (id: string) => void;
}

export function WorkoutLogItem({ workoutLog, onDelete }: WorkoutLogItemProps) {
  return (
    <div className="workout-log-item">
      <div className="workout-info">
        <h3>{workoutLog.exerciseType}</h3>
        <p>{workoutLog.durationMinutes} min</p>
        <p>{workoutLog.caloriesBurned} kcal</p>
      </div>
      <button 
        onClick={() => onDelete(workoutLog.id)}
        className="delete-btn"
      >
        Delete
      </button>
    </div>
  );
}
```

#### Krok 5: Page Integration
```
frontend/src/pages/
└── WorkoutPage.tsx
```

**Przykład - Page:**
```typescript
/**
 * [CO] Strona workout logs
 * [DLACZEGO] Główny widok dla workout feature
 */

import { useState } from 'react';
import { useWorkout } from '@/features/workout/hooks/useWorkout';
import { WorkoutLogList } from '@/features/workout/components/WorkoutLogList';
import { AddWorkoutForm } from '@/features/workout/components/AddWorkoutForm';

export function WorkoutPage() {
  const [selectedDate, setSelectedDate] = useState(new Date());
  const { workoutLogs, isLoading, addWorkout, deleteWorkout } = useWorkout(selectedDate);

  if (isLoading) return <LoadingSpinner />;

  return (
    <div className="workout-page">
      <h1>Workout Tracker</h1>
      
      <DatePicker 
        value={selectedDate} 
        onChange={setSelectedDate} 
      />
      
      <AddWorkoutForm onSubmit={addWorkout} />
      
      <WorkoutLogList 
        workoutLogs={workoutLogs} 
        onDelete={deleteWorkout} 
      />
    </div>
  );
}
```

### 4. Testing

#### Backend - Unit Tests
```csharp
/// <summary>
/// Testy dla AddWorkoutLogCommandHandler
/// </summary>
public class AddWorkoutLogCommandHandlerTests
{
    [Fact]
    public async Task Handle_ValidCommand_CreatesWorkoutLog()
    {
        // Arrange
        var repository = Substitute.For<IWorkoutLogRepository>();
        var logger = Substitute.For<ILogger<AddWorkoutLogCommandHandler>>();
        var handler = new AddWorkoutLogCommandHandler(repository, logger);
        
        var command = new AddWorkoutLogCommand
        {
            UserId = Guid.NewGuid(),
            Date = DateTime.UtcNow,
            ExerciseType = "Running",
            DurationMinutes = 30,
            CaloriesBurned = 300
        };
        
        // Act
        var result = await handler.Handle(command, CancellationToken.None);
        
        // Assert
        Assert.NotEqual(Guid.Empty, result);
        await repository.Received(1).AddAsync(Arg.Any<WorkoutLog>());
    }
}
```

#### Frontend - Component Tests
```typescript
/**
 * Testy dla WorkoutLogItem component
 */

import { render, screen, fireEvent } from '@testing-library/react';
import { WorkoutLogItem } from './WorkoutLogItem';

describe('WorkoutLogItem', () => {
  const mockWorkoutLog = {
    id: '1',
    date: '2026-04-28',
    exerciseType: 'Running',
    durationMinutes: 30,
    caloriesBurned: 300,
  };

  it('renders workout log information', () => {
    render(<WorkoutLogItem workoutLog={mockWorkoutLog} onDelete={() => {}} />);
    
    expect(screen.getByText('Running')).toBeInTheDocument();
    expect(screen.getByText('30 min')).toBeInTheDocument();
    expect(screen.getByText('300 kcal')).toBeInTheDocument();
  });

  it('calls onDelete when delete button is clicked', () => {
    const onDelete = jest.fn();
    render(<WorkoutLogItem workoutLog={mockWorkoutLog} onDelete={onDelete} />);
    
    fireEvent.click(screen.getByText('Delete'));
    
    expect(onDelete).toHaveBeenCalledWith('1');
  });
});
```

### 5. Checklist Implementacji

#### Przed rozpoczęciem
- [ ] Przeczytaj README.md (sprawdź iterację)
- [ ] Przeczytaj .clinerules/rules.md
- [ ] Sprawdź UML/plantUML-kod.txt
- [ ] Wyszukaj podobny kod (Ctrl+Shift+F)
- [ ] Zaplanuj strukturę plików

#### Backend
- [ ] Domain: Entity/Value Object/Interface
- [ ] Application: Command/Query + Handler + Validator
- [ ] Infrastructure: Repository implementation
- [ ] API: Controller endpoint
- [ ] DI: Rejestracja serwisów
- [ ] Tests: Unit tests dla handlers

#### Frontend
- [ ] Types: TypeScript interfaces
- [ ] API: API client functions
- [ ] Hooks: Custom hook z TanStack Query
- [ ] Components: UI components
- [ ] Page: Integration w stronie
- [ ] Tests: Component tests

#### Po implementacji
- [ ] `dotnet build` - kompilacja bez błędów
- [ ] `dotnet test` - wszystkie testy przechodzą
- [ ] `npm run type-check` - brak błędów TypeScript
- [ ] Brak console.log w kodzie
- [ ] Komentarze dodane (CO, DLACZEGO, JAK)
- [ ] Dokumentacja zaktualizowana

## Przykład użycia

```
User: "Dodaj feature do logowania treningów"

Cline (używając skill feature):
1. Czyta README.md → sprawdza iterację
2. Czyta .clinerules/rules.md → standardy
3. Czyta UML/plantUML-kod.txt → architektura
4. Implementuje według workflow:
   - Domain: WorkoutLog entity
   - Application: Commands/Queries
   - Infrastructure: Repository
   - API: Controller
   - Frontend: Types, API, Hooks, Components
5. Dodaje testy
6. Weryfikuje build i testy
```

## Uwagi końcowe

- **Przestrzegaj Clean Architecture** - warstwy nie mogą mieć nieprawidłowych zależności
- **CQRS Pattern** - Commands do zapisu, Queries do odczytu
- **Komentarze są OBOWIĄZKOWE** - CO, DLACZEGO, JAK
- **Testy są OBOWIĄZKOWE** - min 70% coverage
- **Małe commity** - każda warstwa = osobny commit
