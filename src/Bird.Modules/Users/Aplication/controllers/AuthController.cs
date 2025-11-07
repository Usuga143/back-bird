using BackBird.Api.src.Bird.Modules.Users.Aplication.Commands.CreateUser;
using BackBird.Api.src.Bird.Modules.Users.Domain.Entities;
using BackBird.Api.src.Bird.Modules.Users.Domain.Interfaces;
using BackBird.Api.src.Bird.Modules.Users.Domain.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BackBird.Api.src.Bird.Modules.Users.Aplication.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _repo;
        private readonly IPasswordHasher _hasher;
        private readonly IJwtService _jwt;

        public AuthController(IUserRepository repo, IPasswordHasher hasher, IJwtService jwt)
        {
            _repo = repo;
            _hasher = hasher;
            _jwt = jwt;
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

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _repo.GetByEmailAsync(request.Email);
            if (user == null) return Unauthorized("Credenciales inválidas.");

            if (!_hasher.Verify(request.Password, user.PasswordHash))
                return Unauthorized("Credenciales inválidas.");

            var token = _jwt.GenerateToken(user);
            return Ok(new
            {
                token,
                expiresIn = 60 * 60 * 2, // 2 horas en segundos
                user = new { id = user.Id, email = user.Email, name = user.Name, role = user.Role.ToString() }
            });
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
