using Microsoft.Extensions.DependencyInjection;
using WebApiPoultryFarm.Application.Users.CreateUser;

namespace WebApiPoultryFarm.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateUserCommandHandler).Assembly));

            return services;
        }
    }
}
