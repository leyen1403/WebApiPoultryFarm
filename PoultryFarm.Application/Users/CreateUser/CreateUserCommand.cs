using MediatR;
using System.ComponentModel.DataAnnotations;
namespace WebApiPoultryFarm.Application.Users.CreateUser
{
    public class CreateUserCommand : IRequest<UserResponse>
    {
        [MinLength(5, ErrorMessage = "Username tối thiểu 5 ký tự")]
        [MaxLength(20, ErrorMessage = "Username tối đa 20 ký tự")]
        [Required(ErrorMessage = "Username không được để trống")]
        public string UserName { get; set; } = null!;

        [MinLength(6, ErrorMessage = "Password tối thiểu 6 ký tự")]
        [MaxLength(50, ErrorMessage = "Password tối đa 50 ký tự")]
        [Required(ErrorMessage = "Password không được để trống")]
        public string Password { get; set; } = null!;

        [Required(ErrorMessage = "Họ tên không được để trống")]
        [MaxLength(100, ErrorMessage = "Họ tên tối đa 100 ký tự")]
        [MinLength(2, ErrorMessage = "Họ tên tối thiểu 2 ký tự")]
        public string FullName { get; set; } = null!;

        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [MaxLength(100, ErrorMessage = "Email tối đa 100 ký tự")]
        public string Email { get; set; } = null!;
    }
}
