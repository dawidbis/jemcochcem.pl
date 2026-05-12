namespace FitApp.Domain.Interfaces;

using FitApp.Domain.Entities;

public interface IMealLogDomainService
{
    void AddItemToLog(MealLog log, MealLogItem item);
    void RecalculateLogTotals(MealLog log);
}