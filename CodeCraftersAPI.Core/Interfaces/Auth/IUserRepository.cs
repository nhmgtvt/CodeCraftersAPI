using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
