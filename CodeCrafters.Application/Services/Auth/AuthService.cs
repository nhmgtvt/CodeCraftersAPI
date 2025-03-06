using CodeCrafters.Application.Entities.Auth;
using CodeCrafters.Domain.Entities.Auth;
using CodeCrafters.Domain.Interfaces.Auth;

namespace CodeCrafters.Application.Services.Auth
{
    public class AuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtProvider _jwtProvider;
        private readonly IOAuthService _oauthService;

        public AuthService(IUserRepository userRepository, IPasswordHasher passwordHasher, IJwtProvider jwtProvider, IOAuthService oauthService)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
            _jwtProvider = jwtProvider ?? throw new ArgumentNullException(nameof(jwtProvider));
            _oauthService = oauthService ?? throw new ArgumentNullException(nameof(oauthService));
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
                throw new ArgumentException("Invalid input");

            var user = await _userRepository.GetUserByEmailAsync(request.Email);
            if (user is null || !_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
                throw new UnauthorizedAccessException("Invalid credentials");

            var token = _jwtProvider.GenerateToken(user);
            return new LoginResponse { Token = token, User = new UserDto { Id = user.Id, Email = user.Email } };
        }

        public string GetOAuthLoginUrl(string provider)
        {
            return _oauthService.GetOAuthLoginUrl(provider);
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
        public async Task<RegisterResponse> RegisterAsync(RegisterRequest request)
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
                throw new ArgumentException("Invalid input");

            // Check if user already exists by email
            var existingUser = await _userRepository.GetUserByEmailAsync(request.Email);
            if (existingUser != null)
                throw new ArgumentException("Email already exists");

            // Hash the password
            var passwordHash = _passwordHasher.HashPassword(request.Password);

            // Create a new user
            var newUser = new User
            {
                Id = Guid.NewGuid(),
                Email = request.Email,
                PasswordHash = passwordHash,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // Save the new user to the database
            await _userRepository.CreateUserAsync(newUser);

            // Generate JWT token
            var token = _jwtProvider.GenerateToken(newUser);

            // Return RegisterResponse
            return new RegisterResponse
            {
                Token = token,
                User = new UserDto { Id = newUser.Id, Email = newUser.Email }
            };
        }

    }
}
