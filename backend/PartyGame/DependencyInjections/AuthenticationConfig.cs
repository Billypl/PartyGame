namespace PartyGame.DependencyInjections
{
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.IdentityModel.Tokens;
    using System.Text;

   
        public static class AuthenticationConfig
        {
            public static IServiceCollection AddAuthenticationConfig(this IServiceCollection services, IConfiguration configuration)
            {

                var authenticationSettings = new AuthenticationSettings();
                configuration.GetSection("Authentication").Bind(authenticationSettings);

                services.Configure<AuthenticationSettings>(configuration.GetSection("Authentication"));

                services.AddAuthentication(options =>
                    {
                        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    })
                    .AddJwtBearer(cfg =>
                    {
                        cfg.RequireHttpsMetadata = false;
                        cfg.SaveToken = true;
                        cfg.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidIssuer = authenticationSettings.JwtIssuer,
                            ValidAudience = authenticationSettings.JwtIssuer,
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authenticationSettings.JwtKey))
                        };
                    });

                return services;
            }
        }
}


