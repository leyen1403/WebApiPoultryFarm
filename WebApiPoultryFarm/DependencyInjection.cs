using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using WebApiPoultryFarm.Api.Filters;
using WebApiPoultryFarm.Api.Models;
using WebApiPoultryFarm.Application;
using WebApiPoultryFarm.Infrastructure;

namespace WebApiPoultryFarm.Api
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPoultryFarmDI(this IServiceCollection services, IConfiguration configuration)
        {
            // Đăng ký tầng Application
            services.AddApplication();

            // Đăng ký tầng Infrastructure
            services.AddInfrastructure(configuration);

            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var errors = context.ModelState
                    .Where(e => e.Value.Errors.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value.Errors.Select(x => x.ErrorMessage).ToArray()
                    );
                    var response = ApiResponse<string>.Fail("Dữ liệu không hợp lệ", errors);

                    return new BadRequestObjectResult(response);
                };
            });

            // Thêm filter bắt BusinessException toàn cục
            services.AddControllers(opt =>
            {
                opt.Filters.Add<BusinessExceptionFilter>();
            });
            return services;
        }
    }
}
