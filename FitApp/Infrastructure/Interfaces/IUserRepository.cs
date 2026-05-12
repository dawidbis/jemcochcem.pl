namespace FitApp.Infrastructure.Interfaces;

using FitApp.Domain.Entities;
using System;
using System.Threading.Tasks;

public interface IUserRepository : IGenericRepository<User>
{
    Task<User?> GetByEmailAsync(string email);
}