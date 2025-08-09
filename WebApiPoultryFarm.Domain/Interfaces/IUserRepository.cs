using WebApiPoultryFarm.Domain.Entities;

namespace WebApiPoultryFarm.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByUserNameAsync(string userName, CancellationToken ct);
        Task<User?> GetByRefreshTokenAsync(string refreshToken, CancellationToken ct);

        Task AddAsync(User user, CancellationToken ct);
        Task UpdateAsync(User user, CancellationToken ct);
        Task<User?> GetByEmailAsync(string email, CancellationToken ct);
    }
}
