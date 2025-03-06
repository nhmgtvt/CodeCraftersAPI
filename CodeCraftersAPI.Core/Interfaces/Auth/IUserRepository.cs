using CodeCrafters.Domain.Entities.Auth;

namespace CodeCrafters.Domain.Interfaces.Auth
{
    public interface IUserRepository
    {
        Task<User?> GetUserByEmailAsync(string email);
        Task<User?> GetUserByProviderAsync(string provider, string providerUserId);
        Task CreateUserAsync(User user);
        Task LinkExternalLoginAsync(ExternalLogin externalLogin);
    }
}
