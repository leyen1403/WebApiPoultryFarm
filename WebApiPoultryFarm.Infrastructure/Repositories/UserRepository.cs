//UserRepository.cs
using Microsoft.EntityFrameworkCore;
using WebApiPoultryFarm.Domain.Entities;
using WebApiPoultryFarm.Domain.Interfaces;
using WebApiPoultryFarm.Infrastructure.Data;

namespace WebApiPoultryFarm.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly PoultryFarmDbContext _context;

        public UserRepository(PoultryFarmDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task AddAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        public async Task<User?> GetByEmailAsync(string email) => await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

        public async Task<User?> GetByUserNameAsync(string userName) => await _context.Users.FirstOrDefaultAsync(u => u.UserName == userName);
    }
}
