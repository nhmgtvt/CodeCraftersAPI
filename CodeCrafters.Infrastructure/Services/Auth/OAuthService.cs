using System.Net.Http.Headers;
using System.Text.Json;
using CodeCrafters.Domain.Interfaces.Auth;
using Microsoft.Extensions.Configuration;

namespace CodeCrafters.Infrastructure.Services.Auth
{
    public class OAuthService : IOAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public OAuthService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public string GetOAuthLoginUrl(string provider)
        {
            var clientId = _configuration[$"OAuth:{provider}:ClientId"]
                ?? throw new InvalidOperationException($"ClientId for {provider} is missing.");
            var redirectUri = _configuration[$"OAuth:{provider}:RedirectUri"]
                ?? throw new InvalidOperationException($"RedirectUri for {provider} is missing.");
            var authUrl = _configuration[$"OAuth:{provider}:AuthUrl"]
                ?? throw new InvalidOperationException($"AuthUrl for {provider} is missing.");

            return $"{authUrl}?client_id={clientId}&redirect_uri={redirectUri}&response_type=code&scope=email";
        }

        public async Task<(string accessToken, string? refreshToken)> ExchangeCodeForTokenAsync(string provider, string code)
        {
            var tokenUrl = _configuration[$"OAuth:{provider}:TokenUrl"]
                ?? throw new InvalidOperationException($"TokenUrl for {provider} is missing.");
            var clientId = _configuration[$"OAuth:{provider}:ClientId"]
                ?? throw new InvalidOperationException($"ClientId for {provider} is missing.");
            var clientSecret = _configuration[$"OAuth:{provider}:ClientSecret"]
                ?? throw new InvalidOperationException($"ClientSecret for {provider} is missing.");
            var redirectUri = _configuration[$"OAuth:{provider}:RedirectUri"]
                ?? throw new InvalidOperationException($"RedirectUri for {provider} is missing.");

            var requestData = new Dictionary<string, string>
            {
                { "client_id", clientId },
                { "client_secret", clientSecret },
                { "code", code },
                { "redirect_uri", redirectUri },
                { "grant_type", "authorization_code" }
            };

            var requestContent = new FormUrlEncodedContent(requestData);
            var response = await _httpClient.PostAsync(tokenUrl, requestContent);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var tokenData = JsonSerializer.Deserialize<OAuthTokenResponse>(responseContent)
                ?? throw new InvalidOperationException("Failed to deserialize OAuth token response.");

            return (tokenData.AccessToken, tokenData.RefreshToken);
        }

        public async Task<(string providerUserId, string email)> GetUserInfoAsync(string provider, string accessToken)
        {
            var userInfoUrl = _configuration[$"OAuth:{provider}:UserInfoUrl"]
                ?? throw new InvalidOperationException($"UserInfoUrl for {provider} is missing.");

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var response = await _httpClient.GetAsync(userInfoUrl);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var userInfo = JsonSerializer.Deserialize<OAuthUserInfoResponse>(responseContent)
                ?? throw new InvalidOperationException("Failed to deserialize OAuth user info response.");

            return (userInfo.Id, userInfo.Email);
        }
    }

    public class OAuthTokenResponse
    {
        public required string AccessToken { get; set; }
        public string? RefreshToken { get; set; }
    }

    public class OAuthUserInfoResponse
    {
        public required string Id { get; set; }
        public required string Email { get; set; }
    }
}
