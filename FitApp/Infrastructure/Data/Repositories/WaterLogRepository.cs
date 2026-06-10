using FitApp.Domain.Entities;
using FitApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FitApp.Infrastructure.Repositories
{
    public class WaterLogRepository : IWaterLogRepository
    {
        private readonly AppDbContext _context;

        public WaterLogRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<WaterLog?> GetByDateAsync(Guid userId, DateTime date)
        {
            var targetDate = date.Date;
            return await _context.WaterLogs
                .FirstOrDefaultAsync(w => w.UserId == userId && w.Date == targetDate);
        }

        public async Task<List<WaterLog>> GetMonthAsync(Guid userId, int year, int month)
        {
            var from = new DateTime(year, month, 1);
            var to = from.AddMonths(1);
            return await _context.WaterLogs
                .Where(w => w.UserId == userId && w.Date >= from && w.Date < to)
                .ToListAsync();
        }

        public async Task AddAsync(WaterLog waterLog)
        {
            await _context.WaterLogs.AddAsync(waterLog);
        }

        public async Task UpdateAsync(WaterLog waterLog)
        {
            _context.WaterLogs.Update(waterLog);
            await Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}