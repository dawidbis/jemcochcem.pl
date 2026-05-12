namespace FitApp.Application.Features.Users;

using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using FitApp.Domain.Entities;
using FitApp.Infrastructure.Interfaces;

// 1. KOMENDA - to co wysyła klient w JSONie
public class CreateUserCommand : IRequest<Guid>
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty; 
    public decimal Weight { get; set; }
    public decimal Height { get; set; }
    public int Age { get; set; }
    public string Gender { get; set; } = string.Empty;
}

// 2. HANDLER - logika zapisu do bazy
public class CreateUserHandler : IRequestHandler<CreateUserCommand, Guid>
{
    private readonly IUserRepository _userRepository;

    public CreateUserHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Guid> Handle(CreateUserCommand request, CancellationToken ct)
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            
            // TODO: W produkcyjnej aplikacji użyj biblioteki takiej jak BCrypt, aby zahaszować hasło!
            // np. PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
            PasswordHash = request.Password, 
            
            Weight = request.Weight,
            Height = request.Height,
            Age = request.Age,
            Gender = request.Gender
        };

        // Zapisujemy użytkownika do bazy (pamiętaj, że AddAsync w Twoim GenericRepository
        // wywołuje już SaveChangesAsync, więc to zadziała od razu!)
        await _userRepository.AddAsync(user);

        return user.Id; // Zwracamy ID, żeby móc go użyć w dzienniku!
    }
}