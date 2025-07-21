using WebApiPoultryFarm.Domain.Entities;

namespace WebApiPoultryFarm.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByUserNameAsync(string userName);
        Task<User?> GetByEmailAsync(string email);
        Task AddAsync(User user);
    }
}
