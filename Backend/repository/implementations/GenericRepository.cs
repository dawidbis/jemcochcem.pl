using FitApp.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FitApp.Infrastructure.Data.Repositories;

public class GenericRepository<T, TId> : IGenericRepository<T, TId> where T : class
{
    protected readonly AppDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public GenericRepository(AppDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public virtual async Task<T?> GetByIdAsync(TId id) => await _dbSet.FindAsync(id);

    public async Task<IEnumerable<T>> GetAllAsync() => await _dbSet.ToListAsync();

    public async Task AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        await SaveChangesAsync(); // V1: Zapisujemy od razu w repozytorium
    }

    public async Task UpdateAsync(T entity)
    {
        _dbSet.Update(entity);
        await SaveChangesAsync();
    }

    public async Task DeleteAsync(T entity)
    {
        _dbSet.Remove(entity);
        await SaveChangesAsync();
    }

    public async Task<bool> SaveChangesAsync() => await _context.SaveChangesAsync() > 0;
}