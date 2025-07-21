using Microsoft.Extensions.DependencyInjection;

namespace WebApiPoultryFarm.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            //services.AddScoped<ChickenService>();
            // Đăng ký các service, handler, validator... tại đây

            return services;
        }
    }
}
