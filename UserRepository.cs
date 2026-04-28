using FitApp.Domain.Entities;
using FitApp.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FitApp.Infrastructure.Data.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(Guid id) => 
        await _context.Users.FindAsync(id);
}