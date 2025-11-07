using BackBird.Api.src.Bird.Modules.Users.Domain.Enums;

namespace BackBird.Api.src.Bird.Modules.Users.Aplication.Commands.CreateUser
{
    public class CreateUserCommand
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public Role Role { get; set; }


    }
}
