using BackBird.Api.src.Bird.Modules.Users.Domain.Interfaces;
using BCrypt.Net;

namespace BackBird.Api.src.Bird.Modules.Users.Infrastructure.Services
{
    public class PasswordHasher : IPasswordHasher
    {
        public string Hash(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool Verify(string password, string hash)
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }
    }
}
