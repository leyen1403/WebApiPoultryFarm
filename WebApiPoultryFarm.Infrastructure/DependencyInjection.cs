using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiPoultryFarm.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            //services.AddScoped<IChickenRepository, ChickenRepository>();
            // Thêm các repository/service khác tại đây

            return services;
        }
    }
}
