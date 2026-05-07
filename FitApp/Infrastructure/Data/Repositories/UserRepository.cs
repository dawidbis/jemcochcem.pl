namespace FitApp.Infrastructure.Data.Repositories;

using FitApp.Domain.Entities;
using FitApp.Infrastructure.Interfaces;

public class UserRepository : GenericRepository<User>, IUserRepository
{
    public UserRepository(AppDbContext context) : base(context) { }
}