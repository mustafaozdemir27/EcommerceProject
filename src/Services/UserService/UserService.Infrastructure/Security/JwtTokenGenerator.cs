// UserService.Infrastructure/Security/JwtTokenGenerator.cs
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options; // IOptions<JwtSettings> için
using Microsoft.IdentityModel.Tokens; // SymmetricSecurityKey, SigningCredentials için
using System.IdentityModel.Tokens.Jwt; // JwtSecurityTokenHandler, JwtRegisteredClaimNames için
using System.Security.Claims;
using System.Text; // Encoding.UTF8 için
using UserService.Application.Configuration; // JwtSettings için
using UserService.Application.Contracts.Security; // IJwtTokenGenerator için
using UserService.Domain; // User entity için

namespace UserService.Infrastructure.Security
{
    public class JwtTokenGenerator : IJwtTokenGenerator
    {
        // ... (constructor aynı kalacak) ...
        private readonly JwtSettings _jwtSettings;
        private readonly ILogger<JwtTokenGenerator> _logger;

        public JwtTokenGenerator(IOptions<JwtSettings> jwtOptions, ILogger<JwtTokenGenerator> logger)
        {
            _jwtSettings = jwtOptions.Value ?? throw new ArgumentNullException(nameof(jwtOptions), "JWT settings cannot be null.");
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _logger.LogWarning("JwtTokenGenerator: Yapılandırmadan okunan JWT Key: '{JwtKey}'", _jwtSettings.Key);
            _logger.LogWarning("JwtTokenGenerator: Okunan Key uzunluğu: {KeyLength}", _jwtSettings.Key?.Length);

            if (string.IsNullOrEmpty(_jwtSettings.Key) || _jwtSettings.Key.Length < 32)
            {
                _logger.LogError("JwtTokenGenerator: JWT Key doğrulaması BAŞARISIZ! Anahtar null/bos veya çok kisa.");
                throw new ArgumentException("JWT secret key must be configured and be sufficiently long.", nameof(_jwtSettings.Key));
            }
            _logger.LogInformation("JwtTokenGenerator: JWT Key doğrulaması BAŞARILI.");
        }

        // Dönüş tipini GeneratedToken olarak güncelle
        public GeneratedToken GenerateToken(User user, IEnumerable<string>? roles = null)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            if (roles != null)
            {
                foreach (var role in roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }
            }

            var tokenExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.DurationInMinutes); // Geçerlilik bitiş zamanını hesapla

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = tokenExpiresAt, // Hesaplanan bitiş zamanını kullan
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience,
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature)
            };

            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(securityToken);

            // GeneratedToken nesnesini oluştur ve döndür
            return new GeneratedToken(tokenString, tokenExpiresAt);
        }
    }
}