using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiPoultryFarm.Application.Users.LoginUser
{
    public record LoginUserCommand(string UserName, string Password)
        : IRequest<LoginUserResult>;

    public record LoginUserResult(
        int UserId,
        string UserName,
        string AccessToken,
        DateTime AccessTokenExpiresAt,
        string RefreshToken,
        DateTime RefreshTokenExpiresAt
    );
}
