using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiPoultryFarm.Domain.Entities;
using WebApiPoultryFarm.Domain.Interfaces;
using WebApiPoultryFarm.Share.Exeptions;

namespace WebApiPoultryFarm.Application.Users.Register
{
    public class RegisterUserHandler : IRequestHandler<RegisterUserCommand, RegisterUserResult>
    {
        private readonly IUserRepository _userRepository;

        public RegisterUserHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<RegisterUserResult> Handle(RegisterUserCommand request, CancellationToken ct)
        {
            // Normalize inputs
            var normalizedUserName = request.UserName.Trim();
            var normalizedEmail = request.Email.Trim().ToLowerInvariant();

            // Basic business validations
            await EnsureUserNameIsUniqueAsync(normalizedUserName, ct);
            await EnsureEmailIsUniqueAsync(normalizedEmail, ct);
            EnsurePasswordPolicy(request.Password);

            // Hash password with BCrypt
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            // Map to domain entity
            var user = new User
            {
                UserName = normalizedUserName,
                Password = passwordHash,
                FullName = request.FullName?.Trim() ?? string.Empty,
                Email = normalizedEmail,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsActive = true
            };

            // Persist via repository abstraction
            await _userRepository.AddAsync(user, ct);

            // Return application DTO (do not return password)
            return new RegisterUserResult(
                user.Id,
                user.UserName,
                user.FullName,
                user.Email,
                user.CreatedAt
            );
        }

        private async Task EnsureUserNameIsUniqueAsync(string userName, CancellationToken ct)
        {
            var existed = await _userRepository.GetByUserNameAsync(userName, ct);
            if (existed is not null)
                throw new BusinessException("Tên đăng nhập đã tồn tại.", code: "USERNAME_EXISTS");
        }

        private async Task EnsureEmailIsUniqueAsync(string email, CancellationToken ct)
        {
            var existed = await _userRepository.GetByEmailAsync(email, ct);
            if (existed is not null)
                throw new BusinessException("Email đã được sử dụng.", code: "EMAIL_EXISTS");
        }

        private void EnsurePasswordPolicy(string password)
        {
            // Simple policy example; adjust to your needs
            if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
                throw new BusinessException("Mật khẩu tối thiểu 6 ký tự.", code: "WEAK_PASSWORD");
        }
    }
}
