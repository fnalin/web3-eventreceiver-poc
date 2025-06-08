using Fansoft.Blockchain.Scout.Api.Blockchain.Listeners;
using Fansoft.Blockchain.Scout.Api.Models;
using Fansoft.Blockchain.Scout.Api.Services;

namespace Fansoft.Blockchain.Scout.Api.Extensions;

public static class DependenciesSetupExtension
{
    public static void AddDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IBlockRepository, BlockRepository>();
        services.Configure<MongoDbOptions>(configuration.GetSection("MongoDb"));
        services.Configure<BlockchainOptions>(configuration.GetSection("Blockchain"));
        services.AddHostedService<BlockListener>();
    }
    
    public static void UseDependencies(this WebApplication app)
    {
        
    }
}