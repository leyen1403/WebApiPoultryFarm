using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WebApiPoultryFarm.Domain.Interfaces;
using WebApiPoultryFarm.Infrastructure.Data;
using WebApiPoultryFarm.Infrastructure.Repositories;

namespace WebApiPoultryFarm.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Đăng ký DbContext với chuỗi kết nối từ cấu hình
            services.AddDbContext<PoultryFarmDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            });

            // Đăng ký các repository
             services.AddScoped<IUserRepository, UserRepository>();

            return services;
        }
    }
}
