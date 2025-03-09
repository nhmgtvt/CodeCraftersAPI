using CodeCrafters.Application.Entities.Auth;
using CodeCrafters.Domain.Entities.Auth;
using CodeCrafters.Domain.Interfaces.Auth;

namespace CodeCrafters.Application.Services.Auth
{
    public class OAuthLoginService
    {
        private readonly IUserRepository _userRepository;
        private readonly IOAuthService _oauthService;
        private readonly IJwtProvider _jwtProvider;

        public OAuthLoginService(IUserRepository userRepository, IOAuthService oauthService, IJwtProvider jwtProvider)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _oauthService = oauthService ?? throw new ArgumentNullException(nameof(oauthService));
            _jwtProvider = jwtProvider ?? throw new ArgumentNullException(nameof(jwtProvider));
        }

        public async Task<LoginResponse> HandleOAuthCallbackAsync(string provider, string code)
        {
            var (accessToken, refreshToken) = await _oauthService.ExchangeCodeForTokenAsync(provider, code);
            var (providerUserId, email) = await _oauthService.GetUserInfoAsync(provider, accessToken);

            var user = await _userRepository.GetUserByProviderAsync(provider, providerUserId);

            if (user is null)
            {
                user = await _userRepository.GetUserByEmailAsync(email);
                if (user is null)
                {
                    user = new User { Id = Guid.NewGuid(), Email = email, PasswordHash = string.Empty };
                    await _userRepository.CreateUserAsync(user);
                }

                await _userRepository.LinkExternalLoginAsync(new ExternalLogin
                {
                    ExternalLoginId = Guid.NewGuid(),
                    Provider = provider,
                    ProviderUserId = providerUserId,
                    UserId = user.Id,
                    User = user
                });
            }

            var token = _jwtProvider.GenerateToken(user);
            return new LoginResponse { Token = token, User = new UserDto { Id = user.Id, Email = user.Email } };
        }
    }
}
