using PartyGame.Extensions;

namespace PartyGame.DependencyInjections
{
    public static class AutoMapperConfig
    {
        public static IServiceCollection AddAutoMapperConfig(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(MappingProfile));
            services.AddAutoMapper(typeof(AutoMapperConfig).Assembly);
            return services;
        }
    }
}
