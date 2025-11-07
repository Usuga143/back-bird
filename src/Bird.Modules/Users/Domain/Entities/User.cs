using BackBird.Api.src.Bird.Modules.Users.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace BackBird.Api.src.Bird.Modules.Users.Domain.Entities
{
    public class User
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }

        public string Email { get; private set; }

        public string PasswordHash { get; private set; }

        public Role Role { get; private set; }
        public User(
            string email,
            string passwordHash,
            string name,
            Role role)
        {
            Id = Guid.NewGuid();
            Email = email;
            PasswordHash = passwordHash;
            Name = name;
            Role = role;
        }

        protected User() { }
        
    }
}
