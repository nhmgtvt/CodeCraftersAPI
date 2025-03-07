using CodeCrafters.Application.Entities.Auth;
using CodeCrafters.Domain.Entities.Auth;
using CodeCrafters.Domain.Interfaces.Auth;

namespace CodeCrafters.Application.Services.Auth
{
    public class RegisterService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtProvider _jwtProvider;

        public RegisterService(IUserRepository userRepository, IPasswordHasher passwordHasher, IJwtProvider jwtProvider)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
            _jwtProvider = jwtProvider ?? throw new ArgumentNullException(nameof(jwtProvider));
        }

        public async Task<RegisterResponse> RegisterAsync(RegisterRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
                throw new ArgumentException("Invalid input");

            var existingUser = await _userRepository.GetUserByEmailAsync(request.Email);
            if (existingUser != null)
                throw new ArgumentException("Email already exists");

            var passwordHash = _passwordHasher.HashPassword(request.Password);

            var newUser = new User
            {
                Id = Guid.NewGuid(),
                Email = request.Email,
                PasswordHash = passwordHash,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _userRepository.CreateUserAsync(newUser);
            var token = _jwtProvider.GenerateToken(newUser);

            return new RegisterResponse
            {
                Token = token,
                User = new UserDto { Id = newUser.Id, Email = newUser.Email }
            };
        }
    }
}
