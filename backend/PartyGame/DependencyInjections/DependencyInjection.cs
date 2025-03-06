using Microsoft.AspNetCore.Identity;
using PartyGame.DependencyInjections;
using PartyGame.Entities;
using PartyGame.Extensions;
using PartyGame.Middleware;
using System.Text.Json.Serialization;

namespace PartyGame.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddDatabaseConfig(configuration)
                .AddCorsConfiguration(configuration)
                .AddAuthenticationConfig(configuration)
                .AddRepositories()
                .AddServices()
                .AddValidatorsConfig()
                .AddAutoMapperConfig();

            services.AddControllers();

            services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });

            services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
            services.AddScoped<ErrorHandlingMiddleware>();
            services.AddScoped<Seeder>();
            services.AddSwaggerGen();

            return services;
        }
    }

}
