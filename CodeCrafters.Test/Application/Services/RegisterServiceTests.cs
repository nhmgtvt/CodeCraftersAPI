using Moq;
using Xunit;
using CodeCrafters.Application.Services.Auth;
using CodeCrafters.Domain.Entities.Auth;
using CodeCrafters.Domain.Interfaces.Auth;
using CodeCrafters.Application.Entities.Auth;

namespace CodeCrafters.Tests.Application.Services.Auth
{
    public class RegisterServiceTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IPasswordHasher> _passwordHasherMock;
        private readonly Mock<IJwtProvider> _jwtProviderMock;
        private readonly RegisterService _registerService;

        public RegisterServiceTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _passwordHasherMock = new Mock<IPasswordHasher>();
            _jwtProviderMock = new Mock<IJwtProvider>();

            _registerService = new RegisterService(
                _userRepositoryMock.Object,
                _passwordHasherMock.Object,
                _jwtProviderMock.Object
            );
        }

        [Fact]
        public async Task RegisterAsync_ValidRequest_CreatesUserAndReturnsToken()
        {
            // Arrange
            var request = new RegisterRequest { Email = "test@example.com", Password = "SecurePassword123" };
            _userRepositoryMock.Setup(repo => repo.GetUserByEmailAsync(request.Email))
                .ReturnsAsync(null as User); // No existing user
            _passwordHasherMock.Setup(hasher => hasher.HashPassword(request.Password))
                .Returns("hashedPassword");
            _jwtProviderMock.Setup(jwt => jwt.GenerateToken(It.IsAny<User>()))
                .Returns("mockedToken");

            // Act
            var result = await _registerService.RegisterAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("mockedToken", result.Token);
            Assert.Equal("test@example.com", result.User.Email);
            _userRepositoryMock.Verify(repo => repo.CreateUserAsync(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public async Task RegisterAsync_EmailAlreadyExists_ThrowsException()
        {
            // Arrange
            var request = new RegisterRequest { Email = "test@example.com", Password = "SecurePassword123" };
            _userRepositoryMock.Setup(repo => repo.GetUserByEmailAsync(request.Email))
                .ReturnsAsync(new User {Id = new Guid(), Email = request.Email, CreatedAt = DateTime.UtcNow, PasswordHash = "" });

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _registerService.RegisterAsync(request));
        }

        [Theory]
        [InlineData("", "password123")] // Missing email
        [InlineData("user@example.com", "")] // Missing password
        [InlineData("", "")] // Both missing
        public async Task RegisterAsync_InvalidInput_ThrowsException(string email, string password)
        {
            // Arrange
            var request = new RegisterRequest { Email = email, Password = password };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _registerService.RegisterAsync(request));
        }
    }
}
