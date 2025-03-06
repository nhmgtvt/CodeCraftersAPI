using CodeCrafters.Domain.Entities.Auth;

namespace CodeCrafters.Domain.Interfaces.Auth
{
    public interface IJwtProvider
    {
        string GenerateToken(User user);
    }
}
