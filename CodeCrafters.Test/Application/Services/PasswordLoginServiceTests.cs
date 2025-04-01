using Moq;
using Xunit;
using CodeCrafters.Application.Services.Auth;
using CodeCrafters.Domain.Entities.Auth;
using CodeCrafters.Domain.Interfaces.Auth;
using CodeCrafters.Application.Entities.Auth;

namespace CodeCrafters.Tests.Application.Services.Auth
{
    public class PasswordLoginServiceTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IPasswordHasher> _passwordHasherMock;
        private readonly Mock<IJwtProvider> _jwtProviderMock;
        private readonly PasswordLoginService _loginService;

        public PasswordLoginServiceTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _passwordHasherMock = new Mock<IPasswordHasher>();
            _jwtProviderMock = new Mock<IJwtProvider>();

            _loginService = new PasswordLoginService(
                _userRepositoryMock.Object,
                _passwordHasherMock.Object,
                _jwtProviderMock.Object
            );
        }

        [Fact]
        public async Task LoginAsync_ValidCredentials_ReturnsToken()
        {
            // Arrange
            var request = new LoginRequest { Email = "test@example.com", Password = "ValidPassword123" };
            var user = new User { Id = Guid.NewGuid(), Email = request.Email, PasswordHash = "hashedPassword" };

            _userRepositoryMock.Setup(repo => repo.GetUserByEmailAsync(request.Email))
                .ReturnsAsync(user);
            _passwordHasherMock.Setup(hasher => hasher.VerifyPassword(request.Password, user.PasswordHash))
                .Returns(true);
            _jwtProviderMock.Setup(jwt => jwt.GenerateToken(user))
                .Returns("mockedToken");

            // Act
            var result = await _loginService.LoginAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("mockedToken", result.Token);
            Assert.Equal(request.Email, result.User.Email);
        }

        [Fact]
        public async Task LoginAsync_UserNotFound_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var request = new LoginRequest { Email = "notfound@example.com", Password = "SomePassword" };

            _userRepositoryMock.Setup(repo => repo.GetUserByEmailAsync(request.Email))
                .ReturnsAsync(null as User);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _loginService.LoginAsync(request));
        }

        [Fact]
        public async Task LoginAsync_InvalidPassword_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var request = new LoginRequest { Email = "test@example.com", Password = "WrongPassword" };
            var user = new User { Id = Guid.NewGuid(), Email = request.Email, PasswordHash = "hashedPassword" };

            _userRepositoryMock.Setup(repo => repo.GetUserByEmailAsync(request.Email))
                .ReturnsAsync(user);
            _passwordHasherMock.Setup(hasher => hasher.VerifyPassword(request.Password, user.PasswordHash))
                .Returns(false);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _loginService.LoginAsync(request));
        }

        [Theory]
        [InlineData("", "password123")] // Missing email
        [InlineData("user@example.com", "")] // Missing password
        [InlineData("", "")] // Both missing
        public async Task LoginAsync_InvalidInput_ThrowsArgumentException(string email, string password)
        {
            // Arrange
            var request = new LoginRequest { Email = email, Password = password };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _loginService.LoginAsync(request));
        }
    }
}
