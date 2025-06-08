using Fansoft.EventReceiver.Api.Blockchain.Listeners;
using Fansoft.EventReceiver.Api.Data;
using Fansoft.EventReceiver.Api.Data.Repositories;
using Fansoft.EventReceiver.Api.Models;
using Fansoft.EventReceiver.Api.Services;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;

namespace Fansoft.EventReceiver.Api.Extensions;

public static class DependenciesSetupExtension
{
    public static void AddDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<INftService, NftService>();
        services.AddSingleton<IEventRepository, EventRepository>();
        services.AddSingleton<INftPurchaseRepository, NftPurchaseRepository>();
        services.AddSingleton<IPurchaseNotificationRepository, PurchaseNotificationRepository>();
        services.Configure<MongoDbOptions>(configuration.GetSection("MongoDb"));
        services.Configure<BlockchainOptions>(configuration.GetSection("Blockchain"));
        var connString = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<EventReceiverDataContext>(options =>
        {
            options.UseMySql(connString, ServerVersion.AutoDetect(connString));
        });
        services.AddTransient<IDigitalTwinRepository, DigitalTwinRepository>();
        services.AddTransient<IApplicationProviderRepository, ApplicationProviderRepository>();

        services.AddHostedService<NftCreatedListener>();
        services.AddHostedService<NftAccessGrantedListener>();
        
       services.Configure<RabbitMqOptions>(configuration.GetSection("RabbitMq"));
       services.AddSingleton<IRabbitMqPublisher, RabbitMqPublisher>();
       
       services.AddHttpClient();
    }
    
    public static void UseDependencies(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<EventReceiverDataContext>();
        dbContext.Database.Migrate();
       
        
    }
}