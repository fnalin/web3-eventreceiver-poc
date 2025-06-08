using Fansoft.AppProvider.Api.Data;
using Fansoft.AppProvider.Api.Data.Repositories;
using Fansoft.AppProvider.Api.Listeners;
using Fansoft.AppProvider.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Fansoft.AppProvider.Api.Extensions;

public static class DepencyInjectionExtension
{
    public static void AddCoreDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        var connStr = configuration.GetConnectionString("DefaultConnection") ??
                      throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");;
        
        services.AddDbContext<EventProcessDataContext>(options =>
            options.UseMySql(connStr,
                new MySqlServerVersion(new Version(8, 0, 0))));
        
        services.AddScoped<IEventProcessRepository,EventProcessRepository>();
        services.Configure<RabbitMqOptions>(configuration.GetSection("RabbitMq"));
        services.Configure<RemoteServicesOption>(configuration.GetSection("RemoteServices"));
        services.AddHostedService<RabbitMqConsumerHostedService>();
        services.AddHostedService<EventFetchWorker>();
        services.AddHttpClient();
    }

    public static void UseCoreDependencies(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<EventProcessDataContext>();
        dbContext.Database.Migrate();
    }
}