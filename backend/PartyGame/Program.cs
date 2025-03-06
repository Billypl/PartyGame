
using PartyGame.Middleware;
using PartyGame.Extensions;
using PartyGame.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);
ConfigureServices(builder);

var app = builder.Build();
await SeedDatabase(app);
ConfigureMiddleware(app);
app.Run();

// Rejestracja usług
void ConfigureServices(WebApplicationBuilder builder)
{
    builder.Services.AddApplicationDependencies(builder.Configuration);

}

// Seedowanie bazy danych
async Task SeedDatabase(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var seeder = scope.ServiceProvider.GetRequiredService<Seeder>();
    await seeder.Seed();
}

// Konfiguracja middleware
void ConfigureMiddleware(WebApplication app)
{
    app.UseCors("AllowSpecificOrigins");
    app.UseMiddleware<ErrorHandlingMiddleware>();
    app.UseAuthentication();
    app.UseHttpsRedirection();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "PartyGame API"));
    app.UseAuthorization();
    app.MapControllers();
}
