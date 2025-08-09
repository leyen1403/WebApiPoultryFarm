using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiPoultryFarm.Application.Abstractions.Authentication;
using WebApiPoultryFarm.Domain.Interfaces;

namespace WebApiPoultryFarm.Application.Users.Refresh
{
    public class RefreshTokenHandler : IRequestHandler<RefreshTokenCommand, RefreshTokenResult>
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtTokenService _jwt;

        public RefreshTokenHandler(IUserRepository userRepository, IJwtTokenService jwt)
        {
            _userRepository = userRepository;
            _jwt = jwt;
        }

        public async Task<RefreshTokenResult> Handle(RefreshTokenCommand request, CancellationToken ct)
        {
            // English: Validate refresh from DB
            var user = await _userRepository.GetByRefreshTokenAsync(request.RefreshToken, ct);
            if (user is null || user.RefreshTokenExpiryTime is null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
                throw new UnauthorizedAccessException("Invalid or expired refresh token.");

            // English: Rotate to a new pair
            var pair = _jwt.CreateTokenPair(user.Id, user.UserName, user.Email);

            user.RefreshToken = pair.RefreshToken;
            user.RefreshTokenExpiryTime = pair.RefreshTokenExpiresAt;
            user.UpdatedAt = DateTime.UtcNow;
            await _userRepository.UpdateAsync(user, ct);

            return new RefreshTokenResult(pair.AccessToken, pair.AccessTokenExpiresAt, pair.RefreshToken, pair.RefreshTokenExpiresAt);
        }
    }
}
