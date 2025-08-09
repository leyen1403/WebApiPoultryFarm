using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiPoultryFarm.Application.Users.Register
{
    public record RegisterUserCommand(
        string UserName,
        string Password,
        string FullName,
        string Email
    ) : IRequest<RegisterUserResult>;

    public record RegisterUserResult(
        int UserId,
        string UserName,
        string FullName,
        string Email,
        DateTime CreatedAt
    );
}
