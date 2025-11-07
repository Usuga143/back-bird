using BackBird.Api.src.Bird.Modules.Users.Domain.Entities;

namespace BackBird.Api.src.Bird.Modules.Users.Domain.Interfaces
{
    public interface IJwtService
    {
        string GenerateToken(User user);
    }
}
