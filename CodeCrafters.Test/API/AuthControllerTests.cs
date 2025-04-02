using CodeCrafters.Application.Entities.Auth;
using CodeCrafters.Application.Services.Auth;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using CodeCrafters.API.Controllers;
using CodeCrafters.Domain.Interfaces.Auth;
using CodeCrafters.Domain.Entities.Auth;

namespace CodeCrafters.Test.UI.API
{
    public class AuthControllerTests
    {
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<IPasswordHasher> _mockPasswordHasher;
        private readonly Mock<IJwtProvider> _mockJwtProvider;
        private readonly Mock<IOAuthService> _mockOAuthService;

        private readonly RegisterService _registerService;
        private readonly PasswordLoginService _passwordLoginService;
        private readonly OAuthLoginService _oauthLoginService;

        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _mockPasswordHasher = new Mock<IPasswordHasher>();
            _mockJwtProvider = new Mock<IJwtProvider>();
            _mockOAuthService = new Mock<IOAuthService>();

            _registerService = new RegisterService(
                _mockUserRepository.Object, _mockPasswordHasher.Object, _mockJwtProvider.Object);

            _passwordLoginService = new PasswordLoginService(
                _mockUserRepository.Object, _mockPasswordHasher.Object, _mockJwtProvider.Object);

            _oauthLoginService = new OAuthLoginService(
                _mockUserRepository.Object, _mockOAuthService.Object, _mockJwtProvider.Object);

            _controller = new AuthController(_registerService, _passwordLoginService, _oauthLoginService);
        }

        [Fact]
        public async Task Login_ValidCredentials_ReturnsOk()
        {
            var request = new LoginRequest { Email = "user@example.com", Password = "password123" };
            var response = new LoginResponse { Token = "valid-token", User = new UserDto { Id = Guid.NewGuid(), Email = "user@example.com" } };

            // Mock repository behavior instead of service
            _mockUserRepository.Setup(repo => repo.GetUserByEmailAsync(request.Email))
                .ReturnsAsync(new User { Id = Guid.NewGuid(), Email = request.Email, PasswordHash = "hashed-password" });

            _mockPasswordHasher.Setup(hasher => hasher.VerifyPassword("password123", "hashed-password"))
                .Returns(true);

            _mockJwtProvider.Setup(jwt => jwt.GenerateToken(It.IsAny<User>()))
                .Returns("valid-token");

            var result = await _controller.Login(request);
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(response.Token, ((LoginResponse)okResult.Value).Token);
        }

        [Fact]
        public async Task Register_ValidRequest_ReturnsOk()
        {
            var request = new RegisterRequest { Email = "user@example.com", Password = "password123" };
            var response = new RegisterResponse { Token = "valid-token", User = new UserDto { Id = Guid.NewGuid(), Email = "user@example.com" } };

            _mockUserRepository.Setup(repo => repo.GetUserByEmailAsync(request.Email)).ReturnsAsync((User)null!);
            _mockPasswordHasher.Setup(hasher => hasher.HashPassword(request.Password)).Returns("hashed-password");
            _mockJwtProvider.Setup(jwt => jwt.GenerateToken(It.IsAny<User>())).Returns("valid-token");

            var result = await _controller.Register(request);
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(response.Token, ((RegisterResponse)okResult.Value).Token);
        }
    }
}
