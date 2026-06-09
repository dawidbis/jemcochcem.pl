using FitApp.Application.Features.Users;
using FitApp.Application.Interfaces;
using FitApp.Infrastructure.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BCrypt.Net;

namespace FitApp.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IUserRepository _userRepository;
        private readonly IJwtService _jwt;

        public UsersController(IMediator mediator, IUserRepository userRepository, IJwtService jwt)
        {
            _mediator = mediator;
            _userRepository = userRepository;
            _jwt = jwt;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterUser([FromBody] CreateUserCommand command)
        {
            var userId = await _mediator.Send(command);
            var user = await _userRepository.GetByIdAsync(userId);

            var accessToken = _jwt.GenerateAccessToken(user!);
            var refreshToken = _jwt.GenerateRefreshToken();
            user!.RefreshToken = refreshToken;
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
            await _userRepository.UpdateAsync(user);

            return Ok(new { accessToken, refreshToken, userId });
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _userRepository.GetByEmailAsync(request.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                return Unauthorized(new { Message = "Nieprawidłowy email lub hasło." });

            var accessToken = _jwt.GenerateAccessToken(user);
            var refreshToken = _jwt.GenerateRefreshToken();
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
            await _userRepository.UpdateAsync(user);

            return Ok(new
            {
                accessToken,
                refreshToken,
                userId = user.Id,
                user.Email,
                user.CurrentStreak,
                user.LongestStreak
            });
        }

        [HttpPost("refresh")]
        [AllowAnonymous]
        public async Task<IActionResult> Refresh([FromBody] RefreshRequest request)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId);
            if (user == null
                || user.RefreshToken != request.RefreshToken
                || user.RefreshTokenExpiry < DateTime.UtcNow)
                return Unauthorized(new { Message = "Sesja wygasła, zaloguj się ponownie." });

            var accessToken = _jwt.GenerateAccessToken(user);
            var refreshToken = _jwt.GenerateRefreshToken();
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
            await _userRepository.UpdateAsync(user);

            return Ok(new { accessToken, refreshToken });
        }

        [HttpPost("{id}/macros")]
        public async Task<IActionResult> GetMacros([FromRoute] Guid id)
        {
            var result = await _mediator.Send(new CalculateMacrosCommand(id));
            return Ok(result);
        }

        [HttpPost("{id}/micros")]
        public async Task<IActionResult> GetMicros([FromRoute] Guid id)
        {
            var result = await _mediator.Send(new CalculateMicrosCommand(id));
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProfile(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return NotFound();
            var currentStreak = user.CurrentStreak;
            if (user.LastStreakUpdate.HasValue && (DateTime.UtcNow.Date - user.LastStreakUpdate.Value.Date).Days > 1)
            {
                currentStreak = 0;
            }
            return Ok(new
            {
                user.Id,
                user.Email,
                user.Weight,
                user.Height,
                user.Age,
                user.Gender,
                user.TargetWeight,
                user.ActivityMultiplier,
                CurrentStreak = currentStreak,
                user.LongestStreak
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProfile(Guid id, [FromBody] UpdateUserProfileRequest request)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return NotFound();

            user.Weight = request.Weight;
            user.Height = request.Height;
            user.Age = request.Age;
            user.Gender = request.Gender;
            user.ActivityMultiplier = request.ActivityMultiplier;

            await _userRepository.UpdateAsync(user);
            return NoContent();
        }

        [HttpPut("{id}/target-weight")]
        public async Task<IActionResult> SetTargetWeight(Guid id, [FromBody] SetTargetWeightRequest request)
        {
            var ok = await _mediator.Send(new SetTargetWeightCommand(id, request.TargetWeight));
            return ok ? NoContent() : NotFound();
        }

        public record UpdateUserProfileRequest(decimal Weight, decimal Height, int Age, string Gender, decimal ActivityMultiplier);
        public record SetTargetWeightRequest(decimal? TargetWeight);
        public record RefreshRequest(Guid UserId, string RefreshToken);
    }

    public record LoginRequest(string Email, string Password);
}