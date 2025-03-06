using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeCrafters.Domain.Interfaces.Auth
{
    public interface IOAuthService
    {
        string GetOAuthLoginUrl(string provider);
        Task<(string accessToken, string? refreshToken)> ExchangeCodeForTokenAsync(string provider, string code);
        Task<(string providerUserId, string email)> GetUserInfoAsync(string provider, string accessToken);
    }
}
