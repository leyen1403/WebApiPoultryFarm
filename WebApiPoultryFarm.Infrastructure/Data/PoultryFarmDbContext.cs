using Microsoft.EntityFrameworkCore;
using WebApiPoultryFarm.Domain.Entities;

namespace WebApiPoultryFarm.Infrastructure.Data
{
    public class PoultryFarmDbContext : DbContext
    {
        public PoultryFarmDbContext(DbContextOptions<PoultryFarmDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; } = null!; 
    }
}
