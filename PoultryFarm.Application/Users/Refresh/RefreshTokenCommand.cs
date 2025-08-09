using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiPoultryFarm.Application.Users.Refresh
{
    public record RefreshTokenCommand(string RefreshToken)
        : IRequest<RefreshTokenResult>;

    public record RefreshTokenResult(
        string AccessToken,
        DateTime AccessTokenExpiresAt,
        string RefreshToken,
        DateTime RefreshTokenExpiresAt
    );
}
