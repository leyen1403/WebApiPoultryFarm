using MediatR;
using WebApiPoultryFarm.Domain.Entities;
using WebApiPoultryFarm.Domain.Interfaces;
using WebApiPoultryFarm.Share.Exeptions;
using WebApiPoultryFarm.Share.Helpers;

namespace WebApiPoultryFarm.Application.Users.CreateUser
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, UserResponse>
    {
        private readonly IUserRepository _userRepo;

        public CreateUserCommandHandler(IUserRepository userRepository)
        {
            _userRepo = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        public async Task<UserResponse> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.UserName))
                throw new BusinessException("Tên đăng nhập không được để trống.");
            if (string.IsNullOrWhiteSpace(request.Password))
                throw new BusinessException("Mật khẩu không được để trống.");
            if (string.IsNullOrWhiteSpace(request.FullName))
                throw new BusinessException("Họ tên không được để trống.");
            if (string.IsNullOrWhiteSpace(request.Email) || !EmailHelper.IsValidEmail(request.Email))
                throw new BusinessException("Email không hợp lệ.");
            if (await _userRepo.GetByUserNameAsync(request.UserName) != null)
                throw new BusinessException("Tên đăng nhập đã tồn tại.");
            if (await _userRepo.GetByEmailAsync(request.Email) != null)
                throw new BusinessException("Email đã tồn tại.");

            var passwordHash = PasswordHelper.HashPassword(request.Password);
            var user = new User
            {
                UserName = request.UserName,
                Password = passwordHash,
                FullName = request.FullName,
                Email = request.Email,                
                CreatedAt = DateTime.UtcNow
            };
            await _userRepo.AddAsync(user);

            return new UserResponse
            {
                Id = user.Id,
                UserName = user.UserName,
                FullName = user.FullName,
                Email = user.Email
            };
        }
    }
}
