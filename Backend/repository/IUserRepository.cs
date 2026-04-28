using FitApp.Domain.Entities;

namespace FitApp.Domain.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id);
}