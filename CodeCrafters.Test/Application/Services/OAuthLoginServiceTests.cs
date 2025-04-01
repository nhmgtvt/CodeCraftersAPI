using Moq;
using Xunit;
using CodeCrafters.Application.Services.Auth;
using CodeCrafters.Domain.Interfaces.Auth;
using CodeCrafters.Application.Entities.Auth;
using CodeCrafters.Domain.Entities.Auth;

public class OAuthLoginServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IOAuthService> _oauthServiceMock;
    private readonly Mock<IJwtProvider> _jwtProviderMock;
    private readonly OAuthLoginService _oauthLoginService;

    public OAuthLoginServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _oauthServiceMock = new Mock<IOAuthService>();
        _jwtProviderMock = new Mock<IJwtProvider>();
        _oauthLoginService = new OAuthLoginService(_userRepositoryMock.Object, _oauthServiceMock.Object, _jwtProviderMock.Object);
    }

    [Fact]
    public async Task HandleOAuthCallbackAsync_NewUser_CreatesUserAndReturnsToken()
    {
        // Arrange
        var provider = "google";
        var code = "authCode";
        var accessToken = "accessToken";
        var refreshToken = "refreshToken";
        var providerUserId = "provider123";
        var email = "user@example.com";
        var newUser = new User { Id = Guid.NewGuid(), Email = email, PasswordHash = string.Empty };
        var token = "jwtToken";

        _oauthServiceMock.Setup(o => o.ExchangeCodeForTokenAsync(provider, code)).ReturnsAsync((accessToken, refreshToken));
        _oauthServiceMock.Setup(o => o.GetUserInfoAsync(provider, accessToken)).ReturnsAsync((providerUserId, email));
        _userRepositoryMock.Setup(r => r.GetUserByProviderAsync(provider, providerUserId)).ReturnsAsync((User)null);
        _userRepositoryMock.Setup(r => r.GetUserByEmailAsync(email)).ReturnsAsync((User)null);
        _userRepositoryMock.Setup(r => r.CreateUserAsync(It.IsAny<User>())).Returns(Task.CompletedTask);
        _userRepositoryMock.Setup(r => r.LinkExternalLoginAsync(It.IsAny<ExternalLogin>())).Returns(Task.CompletedTask);
        _jwtProviderMock.Setup(j => j.GenerateToken(It.IsAny<User>())).Returns(token);

        // Act
        var response = await _oauthLoginService.HandleOAuthCallbackAsync(provider, code);

        // Assert
        Assert.NotNull(response);
        Assert.Equal(email, response.User.Email);
        Assert.Equal(token, response.Token);
    }

    [Fact]
    public async Task HandleOAuthCallbackAsync_ExistingUserByProvider_ReturnsToken()
    {
        // Arrange
        var provider = "google";
        var code = "authCode";
        var accessToken = "accessToken";
        var refreshToken = "refreshToken";
        var providerUserId = "provider123";
        var email = "user@example.com";
        var existingUser = new User { Id = Guid.NewGuid(), Email = email, PasswordHash = string.Empty };
        var token = "jwtToken";

        _oauthServiceMock.Setup(o => o.ExchangeCodeForTokenAsync(provider, code)).ReturnsAsync((accessToken, refreshToken));
        _oauthServiceMock.Setup(o => o.GetUserInfoAsync(provider, accessToken)).ReturnsAsync((providerUserId, email));
        _userRepositoryMock.Setup(r => r.GetUserByProviderAsync(provider, providerUserId)).ReturnsAsync(existingUser);
        _jwtProviderMock.Setup(j => j.GenerateToken(existingUser)).Returns(token);

        // Act
        var response = await _oauthLoginService.HandleOAuthCallbackAsync(provider, code);

        // Assert
        Assert.NotNull(response);
        Assert.Equal(email, response.User.Email);
        Assert.Equal(token, response.Token);
    }
}
