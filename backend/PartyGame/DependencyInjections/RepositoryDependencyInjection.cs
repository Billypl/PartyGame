using Microsoft.AspNetCore.DataProtection.Repositories;
using PartyGame.Repositories;

namespace PartyGame.DependencyInjection
{
    public static class RepositoryDependencyInjection
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IPlacesRepository, PlacesRepository>();
            services.AddScoped<IScoreboardRepository, ScoreboardRepository>();
            services.AddScoped<IGameSessionRepository, GameSessionRepository>();
            services.AddScoped<IPlacesRepository, PlacesRepository>();
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<IPlacesToCheckRepository, PlacesToCheckRepository>();

            return services;
        }
    }
}
