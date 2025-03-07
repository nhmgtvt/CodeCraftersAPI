using CodeCrafters.Application.Entities.Auth;
using CodeCrafters.Domain.Interfaces.Auth;

namespace CodeCrafters.Application.Services.Auth
{
    public class PasswordLoginService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtProvider _jwtProvider;

        public PasswordLoginService(IUserRepository userRepository, IPasswordHasher passwordHasher, IJwtProvider jwtProvider)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
            _jwtProvider = jwtProvider ?? throw new ArgumentNullException(nameof(jwtProvider));
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
    }
}
