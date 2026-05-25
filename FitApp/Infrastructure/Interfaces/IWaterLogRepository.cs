using FitApp.Domain.Entities;

public interface IWaterLogRepository
{
    Task<WaterLog> GetByDateAsync(Guid userId, DateTime date);
    Task AddAsync(WaterLog waterLog);
    Task UpdateAsync(WaterLog waterLog);
    Task SaveChangesAsync();
}