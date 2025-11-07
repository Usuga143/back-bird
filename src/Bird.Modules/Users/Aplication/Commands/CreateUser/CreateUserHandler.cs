using BackBird.Api.src.Bird.Modules.Users.Domain.Entities;
using BackBird.Api.src.Bird.Modules.Users.Domain.Interfaces;
using BackBird.Api.src.Bird.Modules.Users.Domain.Repositories;
using Microsoft.AspNetCore.Identity;

namespace BackBird.Api.src.Bird.Modules.Users.Aplication.Commands.CreateUser
{
    public class CreateUserHandler
    {
        private readonly IUserRepository _repo;
        private readonly IPasswordHasher _hasher;

        public CreateUserHandler(IUserRepository repo, IPasswordHasher hasher)
        {
            _repo = repo;
            _hasher = hasher;
        }

        public async Task Handle(CreateUserCommand command)
        {
            var existing = await _repo.GetByEmailAsync(command.Email);
            if (existing != null) throw new Exception("El correo electronico ya existe");
            
            var hash = _hasher.Hash(command.Password);
            var user = new User(command.Email, hash, command.Name, command.Role);
            await _repo.AddAsync(user);

        }
    }
}
