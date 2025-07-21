using MediatR;
using WebApiPoultryFarm.Domain.Interfaces;
using WebApiPoultryFarm.Share.Exeptions;
using WebApiPoultryFarm.Share.Helpers;

namespace WebApiPoultryFarm.Application.Users.LoginUser
{
    public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, UserResponse>
    {
        private readonly IUserRepository _userRepo;
        public LoginUserCommandHandler(IUserRepository userRepository)
        {
            _userRepo = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }
        public Task<UserResponse> Handle(LoginUserCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.UserName))
                throw new BusinessException("Tên đăng nhập không được để trống.");
            if (string.IsNullOrWhiteSpace(request.Password))
                throw new BusinessException("Mật khẩu không được để trống.");
            var user = _userRepo.GetByUserNameAsync(request.UserName).Result;
            if (user == null)
                throw new BusinessException("Tên đăng nhập hoặc mật khẩu không đúng.");
            if (!PasswordHelper.VerifyPassword(request.Password, user.Password))
                throw new BusinessException("Tên đăng nhập hoặc mật khẩu không đúng.");
            return Task.FromResult(new UserResponse
            {
                Id = user.Id,
                UserName = user.UserName,
                FullName = user.FullName,
                Email = user.Email
            });
        }
    }
}
