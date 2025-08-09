using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using WebApiPoultryFarm.Application.Abstractions.Authentication;
using WebApiPoultryFarm.Application.Models;

namespace WebApiPoultryFarm.Infrastructure.Auth
{
    public class JwtTokenService : IJwtTokenService
    {
        private readonly JwtOptions _options;
        private readonly SigningCredentials _signingCredentials;

        public JwtTokenService(IOptions<JwtOptions> options)
        {
            _options = options.Value;
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SigningKey));
            _signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        }

        public TokenPair CreateTokenPair(int userId, string userName, string? email)
        {
            var accessExpires = DateTime.UtcNow.AddMinutes(_options.AccessTokenMinutes);
            var accessToken = BuildAccessToken(userId, userName, email, accessExpires);

            var refreshToken = GenerateRefreshToken();
            var refreshExpires = DateTime.UtcNow.AddDays(_options.RefreshTokenDays);

            return new TokenPair(accessToken, accessExpires, refreshToken, refreshExpires);
        }

        public TokenPair CreateAccessToken(int userId, string userName, string? email, string existingRefreshToken, DateTime existingRefreshExpiry)
        {
            var accessExpires = DateTime.UtcNow.AddMinutes(_options.AccessTokenMinutes);
            var accessToken = BuildAccessToken(userId, userName, email, accessExpires);
            return new TokenPair(accessToken, accessExpires, existingRefreshToken, existingRefreshExpiry);
        }

        public string GenerateRefreshToken()
        {
            // Use cryptographically strong random bytes
            var bytes = new byte[64];
            RandomNumberGenerator.Fill(bytes);
            return Convert.ToBase64String(bytes);
        }

        private string BuildAccessToken(int userId, string userName, string? email, DateTime expiresAtUtc)
        {
            // Minimal claims; add roles if you need later
            var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new("uid", userId.ToString()),
            new("uname", userName),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
            if (!string.IsNullOrWhiteSpace(email))
                claims.Add(new Claim(JwtRegisteredClaimNames.Email, email!));

            var token = new JwtSecurityToken(
                issuer: _options.Issuer,
                audience: _options.Audience,
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: expiresAtUtc,
                signingCredentials: _signingCredentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
