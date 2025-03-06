namespace CodeCrafters.Domain.Interfaces.Auth
{
    public interface IPasswordHasher
    {
        bool VerifyPassword(string password, string hashedPassword);
        string HashPassword(string password);
    }
}
