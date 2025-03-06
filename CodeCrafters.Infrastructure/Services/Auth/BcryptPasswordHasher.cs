using CodeCrafters.Domain.Interfaces.Auth;

namespace CodeCrafters.Infrastructure.Services.Auth
{
    public class BcryptPasswordHasher : IPasswordHasher
    {
        public bool VerifyPassword(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }
    }
}
