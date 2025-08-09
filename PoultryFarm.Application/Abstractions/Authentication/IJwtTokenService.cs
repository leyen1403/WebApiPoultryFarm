using WebApiPoultryFarm.Application.Models;

namespace WebApiPoultryFarm.Application.Abstractions.Authentication
{
    public interface IJwtTokenService
    {
        // Issue both access & refresh tokens for user with int id
        TokenPair CreateTokenPair(int userId, string userName, string? email);

        // Re-issue only access token while keeping an existing refresh token (optional)
        TokenPair CreateAccessToken(int userId, string userName, string? email, string existingRefreshToken, DateTime existingRefreshExpiry);

        // Create a cryptographically-strong refresh token string
        string GenerateRefreshToken();
    }
}
