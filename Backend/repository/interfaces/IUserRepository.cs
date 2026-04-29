using FitApp.Domain.Entities;

namespace FitApp.Domain.Interfaces;

public interface IUserRepository : IGenericRepository<User, Guid>
{
}