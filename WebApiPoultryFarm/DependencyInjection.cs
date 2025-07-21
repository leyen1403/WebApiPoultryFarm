using WebApiPoultryFarm.Application;
using WebApiPoultryFarm.Infrastructure;

namespace WebApiPoultryFarm.Api
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPoultryFarmDI(this IServiceCollection services)
        {
            // Đăng ký tầng Application
            services.AddApplication();

            // Đăng ký tầng Infrastructure
            services.AddInfrastructure();

            // Đăng ký thêm các DI khác nếu có (ex: Shared, ExternalService...)

            return services;
        }
    }
}
