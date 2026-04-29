using System;
using FitApp.Domain.Entities;
using FitApp.Domain.Interfaces;

namespace FitApp.Infrastructure.Data.Repositories;
public class UserRepository : GenericRepository<User, Guid>, IUserRepository
{
    public UserRepository(AppDbContext context) : base(context) { }
}