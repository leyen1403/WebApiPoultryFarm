//UserRepository.cs
using Microsoft.EntityFrameworkCore;
using WebApiPoultryFarm.Domain.Entities;
using WebApiPoultryFarm.Domain.Interfaces;
using WebApiPoultryFarm.Infrastructure.Data;

namespace WebApiPoultryFarm.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly PoultryFarmDbContext _db;
        public UserRepository(PoultryFarmDbContext db) => _db = db;

        public async Task<User?> GetByUserNameAsync(string userName, CancellationToken ct)
            => await _db.Users.FirstOrDefaultAsync(u => u.UserName == userName, ct);

        public async Task<User?> GetByRefreshTokenAsync(string refreshToken, CancellationToken ct)
            => await _db.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken, ct);

        public async Task AddAsync(User user, CancellationToken ct)
        {
            await _db.Users.AddAsync(user, ct);
            await _db.SaveChangesAsync(ct);
        }

        public async Task UpdateAsync(User user, CancellationToken ct)
        {
            _db.Users.Update(user);
            await _db.SaveChangesAsync(ct);
        }

        public async Task<User?> GetByEmailAsync(string email, CancellationToken ct)
            => await _db.Users.FirstOrDefaultAsync(u => u.Email == email, ct);
    }
}
