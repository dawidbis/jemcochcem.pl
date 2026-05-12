namespace FitApp.Application.Features.Diet;

using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FitApp.Infrastructure.Interfaces;
using FitApp.Domain.Interfaces;

// Komenda potrzebuje: Kto usuwa, z jakiego dnia i ID konkretnego posiłku
public record DeleteMealItemCommand(Guid UserId, DateTime Date, Guid ItemId) : IRequest<Unit>;

public class DeleteMealItemHandler : IRequestHandler<DeleteMealItemCommand, Unit>
{
    private readonly IMealLogRepository _mealLogRepository;
    private readonly IMealLogDomainService _mealLogService;

    public DeleteMealItemHandler(
        IMealLogRepository mealLogRepository, 
        IMealLogDomainService mealLogService)
    {
        _mealLogRepository = mealLogRepository;
        _mealLogService = mealLogService;
    }

    public async Task<Unit> Handle(DeleteMealItemCommand request, CancellationToken ct)
    {
        // 1. Pobieramy dziennik z bazy
        var log = await _mealLogRepository.GetByDateAsync(request.UserId, request.Date);
        if (log == null) throw new ArgumentException("Dziennik nie istnieje.");

        // 2. Szukamy posiłku do usunięcia na liście
        var itemToRemove = log.Items.FirstOrDefault(i => i.Id == request.ItemId);
        if (itemToRemove == null) throw new ArgumentException("Nie znaleziono posiłku o podanym ID.");

        // 3. Usuwamy element z listy
        log.Items.Remove(itemToRemove);

        // 4. MAGIA: Przeliczamy ponownie kalorie dla całego dziennika (bo ubyło jedzenia!)
        _mealLogService.RecalculateLogTotals(log);

        // 5. Zapisujemy zmiany do bazy
        await _mealLogRepository.UpdateAsync(log);

        return Unit.Value;
    }
}