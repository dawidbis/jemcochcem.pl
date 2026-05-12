namespace FitApp.Application.Features.Diet;

using MediatR;
using FitApp.Domain.Interfaces; // <-- Pamiętaj o usingach dla interfejsów!
using System;
using System.Threading;
using System.Threading.Tasks;
using FitApp.Infrastructure.Interfaces;
using FitApp.Domain.Entities;

public class AddMealItemCommand : IRequest<Unit>
{
    public Guid UserId { get; set; }
    public DateTime Date { get; set; }
    public Guid FoodProductId { get; set; }
    public decimal Grams { get; set; }
}

public class AddMealItemHandler : IRequestHandler<AddMealItemCommand, Unit>
{
    private readonly IMealLogRepository _mealLogRepository;
    private readonly IFoodRepository _foodRepository;
    
    // POPRAWKA 1: Używamy INTERFEJSU zamiast konkretnej klasy
    private readonly IMealLogDomainService _mealLogService;

    public AddMealItemHandler(
        IMealLogRepository mealLogRepository, 
        IFoodRepository foodRepository, 
        IMealLogDomainService mealLogService) // <-- Wstrzykujemy interfejs
    {
        _mealLogRepository = mealLogRepository;
        _foodRepository = foodRepository;
        _mealLogService = mealLogService;
    }

    public async Task<Unit> Handle(AddMealItemCommand request, CancellationToken ct)
    {
        // POPRAWKA 2: Bezpieczne sprawdzanie, czy tworzymy nowy dziennik
        bool isNewLog = false;
        var log = await _mealLogRepository.GetByDateAsync(request.UserId, request.Date);
        
        if (log == null)
        {
            log = new MealLog { Id = Guid.NewGuid(), UserId = request.UserId, Date = request.Date };
            isNewLog = true; // Zaznaczamy, że to świeżynka
        }

        var food = await _foodRepository.GetByIdAsync(request.FoodProductId);
        if (food == null) throw new ArgumentException("Food product not found.");

        var item = new MealLogItem 
        {  
            MealLogId = log.Id, 
            FoodProductId = food.Id, 
            Grams = request.Grams, 
            FoodProduct = food 
        };
        
        // Użycie usługi domenowej - to miałeś idealnie!
        _mealLogService.AddItemToLog(log, item);

        if (isNewLog) 
        {
            await _mealLogRepository.AddAsync(log);
        }
        else 
        {
            // Upewnij się, że Twoje repozytorium nie tworzy nowego obiektu w metodzie Update,
            // tylko wywołuje _context.Entry(entity).State = EntityState.Modified;
            await _mealLogRepository.UpdateAsync(log);
        }
        return Unit.Value;
    }
}