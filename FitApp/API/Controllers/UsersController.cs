using FitApp.Application.Features.Users;
using FitApp.Infrastructure.Interfaces; // Dla IUserRepository
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace FitApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IUserRepository _userRepository;

        // Wstrzykujemy oba potrzebne serwisy
        public UsersController(IMediator mediator, IUserRepository userRepository)
        {
            _mediator = mediator;
            _userRepository = userRepository;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] CreateUserCommand command)
        {
            var userId = await _mediator.Send(command);
            return Ok(new { Id = userId });
        }

        [HttpPost("{id}/macros")]
        public async Task<IActionResult> GetMacros([FromRoute] Guid id, [FromBody] decimal activityMultiplier)
        {
            var result = await _mediator.Send(new CalculateMacrosCommand(id, activityMultiplier));
            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            // 1. Szukamy użytkownika po mailu
            var user = await _userRepository.GetByEmailAsync(request.Email);

            // 2. Porównujemy hasła (używamy Twojego pola PasswordHash)
            // UWAGA: Docelowo tutaj powinno być VerifyHash(request.Password, user.PasswordHash)
            if (user == null || user.PasswordHash != request.Password)
        {
            return Unauthorized(new { Message = "Nieprawidłowy email lub hasło." });
        }

        // 3. Zwracamy dane (zamiast user.Name używamy user.Email, bo Name nie istnieje w encji)
        return Ok(new 
        { 
            UserId = user.Id, 
            UserEmail = user.Email,
            Message = "Zalogowano pomyślnie!" 
        });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProfile(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return NotFound();

            // Zwracamy obiekt profilu
            return Ok(new {
                user.Id,
                user.Email,
                user.Weight,
                user.Height,
                user.Age,
                user.Gender
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProfile(Guid id, [FromBody] UpdateUserProfileRequest request)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return NotFound();

            // Aktualizujemy dane z Twojej encji
            user.Weight = request.Weight;
            user.Height = request.Height;
            user.Age = request.Age;
            user.Gender = request.Gender;

            await _userRepository.UpdateAsync(user);
            return NoContent();
        }

// Pomocniczy rekord dla edycji
    public record UpdateUserProfileRequest(decimal Weight, decimal Height, int Age, string Gender);
    }

    // Model pomocniczy poza klasą kontrolera (lub w oddzielnym pliku)
    public record LoginRequest(string Email, string Password);
}