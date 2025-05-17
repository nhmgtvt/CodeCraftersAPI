using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CodeCrafters.Domain.Entities.Auth;
using CodeCrafters.Domain.Interfaces.Auth;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;


namespace CodeCrafters.Infrastructure.Services.Auth
{
    public class JwtProvider : IJwtProvider
    {
        private readonly string _secretKey;
        private readonly int _expiryMinutes;

        public JwtProvider(IConfiguration configuration)
        {
            _secretKey = configuration["JwtSettings:SecretKey"]
                         ?? throw new ArgumentNullException("JWT secret key is missing in configuration.");
            _expiryMinutes = configuration.GetValue<int>("JwtSettings:ExpiryMinutes", 60); // Default 60 minutes
        }

        public string GenerateToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_secretKey);

            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) // Unique token ID
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(_expiryMinutes),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
