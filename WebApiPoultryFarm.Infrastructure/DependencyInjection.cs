using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using WebApiPoultryFarm.Application.Abstractions.Authentication;
using WebApiPoultryFarm.Domain.Interfaces;
using WebApiPoultryFarm.Infrastructure.Auth;
using WebApiPoultryFarm.Infrastructure.Data;
using WebApiPoultryFarm.Infrastructure.Repositories;

namespace WebApiPoultryFarm.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Database
            services.AddDbContext<PoultryFarmDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            });

            // Repositories
            services.AddScoped<IUserRepository, UserRepository>();

            // JWT (options + service + bearer)
            RegisterJwt(services, configuration);

            return services;
        }

        private static void RegisterJwt(IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<JwtOptions>(configuration.GetSection("Jwt"));
            services.AddSingleton<IJwtTokenService, JwtTokenService>();

            var jwt = configuration.GetSection("Jwt");
            var signingKey = jwt.GetValue<string>("SigningKey") ?? string.Empty;

            if (string.IsNullOrWhiteSpace(signingKey) || signingKey.Length < 32)
                throw new InvalidOperationException("Jwt:SigningKey must be provided and at least 32 characters.");

            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(opt =>
                {
                    opt.TokenValidationParameters = new()
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwt.GetValue<string>("Issuer"),
                        ValidAudience = jwt.GetValue<string>("Audience"),
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey)),
                        ClockSkew = TimeSpan.FromSeconds(30)
                    };

                    // English: Normalize token source (header/query/cookie) and handle double "Bearer " paste
                    opt.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = ctx =>
                        {
                            // 1) From Authorization header
                            var auth = ctx.Request.Headers["Authorization"].FirstOrDefault();
                            if (!string.IsNullOrWhiteSpace(auth))
                            {
                                // English: Accept both "Bearer <token>" and "<token>"
                                var token = auth.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)
                                    ? auth.Substring("Bearer ".Length).Trim()
                                    : auth.Trim();

                                // English: If user pasted "Bearer <token>" into Swagger, header becomes "Bearer Bearer <token>"
                                if (token.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                                    token = token.Substring("Bearer ".Length).Trim();

                                if (!string.IsNullOrWhiteSpace(token))
                                    ctx.Token = token;
                            }

                            // 2) Fallback: query string ?access_token=
                            if (string.IsNullOrEmpty(ctx.Token))
                            {
                                var qsToken = ctx.Request.Query["access_token"].FirstOrDefault();
                                if (!string.IsNullOrWhiteSpace(qsToken))
                                    ctx.Token = qsToken;
                            }

                            // 3) Fallback: cookie "access_token"
                            if (string.IsNullOrEmpty(ctx.Token))
                            {
                                if (ctx.Request.Cookies.TryGetValue("access_token", out var cookieToken) &&
                                    !string.IsNullOrWhiteSpace(cookieToken))
                                {
                                    ctx.Token = cookieToken;
                                }
                            }

                            return Task.CompletedTask;
                        },

                        OnChallenge = context =>
                        {
                            // English: Return JSON instead of empty 401; also mark expired via header
                            context.HandleResponse();
                            var isExpired = context.AuthenticateFailure is SecurityTokenExpiredException;

                            if (isExpired)
                                context.Response.Headers.Append("Token-Expired", "true");

                            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            context.Response.ContentType = "application/json";

                            var payload = new
                            {
                                success = false,
                                message = isExpired
                                    ? "Unauthorized: access token has expired."
                                    : "Unauthorized: missing or invalid access token.",
                                data = (object?)null,
                                errors = (object?)null
                            };
                            return context.Response.WriteAsJsonAsync(payload);
                        },

                        OnForbidden = context =>
                        {
                            context.Response.StatusCode = StatusCodes.Status403Forbidden;
                            context.Response.ContentType = "application/json";
                            var payload = new
                            {
                                success = false,
                                message = "Forbidden: insufficient permissions.",
                                data = (object?)null,
                                errors = (object?)null
                            };
                            return context.Response.WriteAsJsonAsync(payload);
                        },

                        OnAuthenticationFailed = ctx =>
                        {
                            // English: Log exact reason (signature, audience, issuer, expired...)
                            Console.WriteLine($"JWT failed: {ctx.Exception.GetType().Name}: {ctx.Exception.Message}");
                            return Task.CompletedTask;
                        }
                    };
                });

            services.AddAuthorization();
        }
    }
}
