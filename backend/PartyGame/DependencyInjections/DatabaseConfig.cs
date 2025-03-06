using MongoDB.Driver;
using PartyGame.Entities;
using System.Runtime.InteropServices;

namespace PartyGame.DependencyInjections
{
    public static class DatabaseConfig
    {
        public static IServiceCollection AddDatabaseConfig(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpContextAccessor();
            services.AddSingleton<IMongoClient>(sp =>
            {
                string connectionString = "";
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    connectionString = configuration.GetConnectionString("MongoDBWindowsConnection");
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    connectionString = configuration.GetConnectionString("MongoDBLinuxConnection");
                }
                return new MongoClient(connectionString);
            });


            services.AddScoped<GameDbContext>();
            services.AddScoped<GameDbContext>();
            return services;
        }
    }
}
