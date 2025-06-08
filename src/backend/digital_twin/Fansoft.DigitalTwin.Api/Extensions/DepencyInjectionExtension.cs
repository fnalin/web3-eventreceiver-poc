using Fansoft.DigitalTwin.Api.Data;
using Fansoft.DigitalTwin.Api.Services;
using Microsoft.EntityFrameworkCore;

namespace Fansoft.DigitalTwin.Api.Extensions;

public static class DepencyInjectionExtension
{
    public static void AddCoreDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        var connStr = configuration.GetConnectionString("DefaultConnection") ??
                      throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");;
        
        services.AddDbContext<AppDbContext>(options =>
            options.UseMySql(connStr,
                new MySqlServerVersion(new Version(8, 0, 0))));
        
        services.AddScoped<IEventApiService,EventApiService>();
        var uri = configuration["RemoteServices:EventURL"] ?? throw new InvalidOperationException("EventApiURL is not configured");
        services.AddHttpClient<EventApiService>(client =>
        {
            client.BaseAddress = new Uri(uri);
        });
    }

    public static void UseCoreDependencies(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        dbContext.Database.Migrate();
    }
}