namespace BackBird.Api.src.Bird.Modules.Users.Domain.Interfaces
{
    public interface IPasswordHasher
    {
        string Hash(string password);
        bool Verify(string password, string hash);
    }
}
