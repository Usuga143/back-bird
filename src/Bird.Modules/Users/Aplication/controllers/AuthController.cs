using BackBird.Api.src.Bird.Modules.Users.Aplication.Commands.CreateUser;
using BackBird.Api.src.Bird.Modules.Users.Domain.Entities;
using BackBird.Api.src.Bird.Modules.Users.Domain.Interfaces;
using BackBird.Api.src.Bird.Modules.Users.Domain.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.Extensions.Logging;

namespace BackBird.Api.src.Bird.Modules.Users.Aplication.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _repo;
        private readonly IPasswordHasher _hasher;
        private readonly IJwtService _jwt;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IUserRepository repo, IPasswordHasher hasher, IJwtService jwt, ILogger<AuthController> logger)
        {
            _repo = repo;
            _hasher = hasher;
            _jwt = jwt;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] CreateUserCommand request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
                return BadRequest("Email y contraseña son obligatorios.");

            var existing = await _repo.GetByEmailAsync(request.Email);
            if (existing != null) return Conflict("El correo electrónico ya está en uso.");

            var hash = _hasher.Hash(request.Password);
            var user = new User(request.Email, hash, request.Name, request.Role);
            await _repo.AddAsync(user);

            return Created(string.Empty, new { id = user.Id, email = user.Email, name = user.Name, role = user.Role.ToString() });
        }

        public class LoginRequest { public string Email { get; set; } = ""; public string Password { get; set; } = ""; }
        public class UserDto { public Guid Id { get; set; } public string Email { get; set; } = ""; public string Name { get; set; } = ""; public string Role { get; set; } = ""; }
        public class LoginResponse { public UserDto User { get; set; } = new UserDto(); public string Token { get; set; } = ""; }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            {
                _logger.LogWarning("Login attempt with invalid payload");
                return BadRequest("Email and password are required.");
            }

            var user = await _repo.GetByEmailAsync(request.Email);
            if (user == null)
            {
                _logger.LogInformation("Login failed for {Email}: user not found", request.Email);
                return Unauthorized();
            }

            if (!_hasher.Verify(request.Password, user.PasswordHash))
            {
                _logger.LogInformation("Login failed for {Email}: invalid password", request.Email);
                return Unauthorized();
            }

            var token = _jwt.GenerateToken(user);

            var resp = new LoginResponse
            {
                Token = token,
                User = new UserDto { Id = user.Id, Email = user.Email, Name = user.Name, Role = user.Role.ToString() }
            };

            return Ok(resp);
        }

        // Ejemplo de endpoint protegido que el frontend puede llamar con Authorization header
        [Authorize]
        [HttpGet("me")]
        public IActionResult Me()
        {
            var id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            return Ok(new { id, email, role });
        }
    }
}
