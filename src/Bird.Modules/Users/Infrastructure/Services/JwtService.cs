using BackBird.Api.src.Bird.Modules.Users.Domain.Entities;
using BackBird.Api.src.Bird.Modules.Users.Domain.Interfaces;
using System;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace BackBird.Api.src.Bird.Modules.Users.Infrastructure.Services
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;
        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateToken(User user)
        {
            var claims = new[] {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            var keyConfig = _configuration["Jwt:Key"] ?? string.Empty;

            // Try to interpret the key as base64; if that fails, use UTF8 bytes
            byte[] keyBytes;
            try
            {
                keyBytes = Convert.FromBase64String(keyConfig);
            }
            catch
            {
                keyBytes = Encoding.UTF8.GetBytes(keyConfig);
            }

            if (keyBytes.Length < 32)
            {
                throw new InvalidOperationException("JWT key is too short. Provide a key with at least 32 bytes (256 bits). Use a 32-byte binary key encoded in base64 or a UTF8 string with 32+ characters.");
            }

            var key = new SymmetricSecurityKey(keyBytes);
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
