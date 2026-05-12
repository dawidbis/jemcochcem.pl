using FitApp.Domain.Entities;
using FitApp.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FitApp.Infrastructure.Data.Repositories;

public class BodyMeasurementRepository : GenericRepository<BodyMeasurement>, IBodyMeasurementRepository
{
    public BodyMeasurementRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<BodyMeasurement>> GetUserHistoryAsync(Guid userId)
    {
        return await _dbSet
            .Where(m => m.UserId == userId)
            .OrderByDescending(m => m.Date)
            .ToListAsync();
    }

    public async Task<BodyMeasurement?> GetLatestAsync(Guid userId)
    {
        return await _dbSet
            .Where(m => m.UserId == userId)
            .OrderByDescending(m => m.Date)
            .FirstOrDefaultAsync();
    }
}