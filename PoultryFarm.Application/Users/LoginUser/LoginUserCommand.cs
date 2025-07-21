using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiPoultryFarm.Application.Users.LoginUser
{
    public class LoginUserCommand : IRequest<UserResponse>
    {
        [MinLength(5, ErrorMessage = "Username tối thiểu 5 ký tự")]
        [MaxLength(20, ErrorMessage = "Username tối đa 20 ký tự")]
        [Required(ErrorMessage = "Username không được để trống")]
        public string UserName { get; set; } = null!;

        [MinLength(6, ErrorMessage = "Password tối thiểu 6 ký tự")]
        [MaxLength(50, ErrorMessage = "Password tối đa 50 ký tự")]
        [Required(ErrorMessage = "Password không được để trống")]
        public string Password { get; set; } = null!;
    }
}
