namespace FitApp.Infrastructure.Data.Repositories;

using FitApp.Domain.Entities;
using FitApp.Domain.Interfaces;

public class UserRepository : GenericRepository<User>, IUserRepository
{
    public UserRepository(AppDbContext context) : base(context) { }
}