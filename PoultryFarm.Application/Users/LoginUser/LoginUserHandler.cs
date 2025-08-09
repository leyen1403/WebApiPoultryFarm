using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiPoultryFarm.Application.Abstractions.Authentication;
using WebApiPoultryFarm.Domain.Interfaces;
using WebApiPoultryFarm.Share.Exeptions;

namespace WebApiPoultryFarm.Application.Users.LoginUser
{
    public class LoginUserHandler : IRequestHandler<LoginUserCommand, LoginUserResult>
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtTokenService _jwt;

        public LoginUserHandler(IUserRepository userRepository, IJwtTokenService jwt)
        {
            _userRepository = userRepository;
            _jwt = jwt;
        }

        public async Task<LoginUserResult> Handle(LoginUserCommand request, CancellationToken ct)
        {
            // Lookup user
            var user = await _userRepository.GetByUserNameAsync(request.UserName, ct);
            if (user is null)
                throw new BusinessException("Tên đăng nhập hoặc mật khẩu không đúng.", code: "AUTH_INVALID");

            // Verify password
            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
                throw new BusinessException("Tên đăng nhập hoặc mật khẩu không đúng.", code: "AUTH_INVALID");

            // Check if user is disabled
            if (!user.IsActive)
                throw new BusinessException("Tài khoản đã bị vô hiệu hoá.", code: "AUTH_DISABLED");

            // Issue token pair
            var pair = _jwt.CreateTokenPair(user.Id, user.UserName, user.Email);

            // Rotate refresh + update audit fields
            user.RefreshToken = pair.RefreshToken;
            user.RefreshTokenExpiryTime = pair.RefreshTokenExpiresAt;
            user.LastLoginAt = DateTime.UtcNow;
            user.UpdatedAt = DateTime.UtcNow;
            await _userRepository.UpdateAsync(user, ct);

            return new LoginUserResult(
                user.Id,
                user.UserName,
                pair.AccessToken,
                pair.AccessTokenExpiresAt,
                pair.RefreshToken,
                pair.RefreshTokenExpiresAt
            );
        }
    }
}
