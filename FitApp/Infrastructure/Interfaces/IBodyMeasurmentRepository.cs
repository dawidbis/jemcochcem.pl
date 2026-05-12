namespace FitApp.Infrastructure.Interfaces;

using FitApp.Domain.Entities;

public interface IBodyMeasurementRepository : IGenericRepository<BodyMeasurement>
{
    Task<IEnumerable<BodyMeasurement>> GetUserHistoryAsync(Guid userId);
    Task<BodyMeasurement?> GetLatestAsync(Guid userId);
}

