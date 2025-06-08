using Fansoft.EventReceiver.Api.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Fansoft.EventReceiver.Api.Data.Repositories;

public interface IPurchaseNotificationRepository
{
    Task<bool> IsSentNotificationAsync(string nftPurchaseEventHash, NotificationType rabbitMq, CancellationToken stoppingToken);
    Task AddNotificationAsync(PurchaseNotification doc, CancellationToken stoppingToken);
}

public class PurchaseNotificationRepository : IPurchaseNotificationRepository
{
    private readonly IMongoCollection<PurchaseNotification> _collection;
    private readonly ILogger<PurchaseNotificationRepository> _logger;

    public PurchaseNotificationRepository(
        IOptions<MongoDbOptions> options, 
        ILogger<PurchaseNotificationRepository> logger)
    {
        _logger = logger;
        if (options == null || string.IsNullOrEmpty(options.Value.ConnectionString) || string.IsNullOrEmpty(options.Value.Database))
        {
            throw new ArgumentException("MongoDB options are not properly configured.");
        }
        var client = new MongoClient(options.Value.ConnectionString);
        var database = client.GetDatabase(options.Value.Database);
        _collection = database.GetCollection<PurchaseNotification>("purchaseNotifications");
    }
    
    public async Task AddNotificationAsync(PurchaseNotification doc, CancellationToken stoppingToken) =>
        await _collection.InsertOneAsync(doc, cancellationToken: stoppingToken);


    public async Task<bool> IsSentNotificationAsync(string nftPurchaseEventHash, NotificationType notificationType, CancellationToken stoppingToken) =>
        await _collection
            .Find(x => x.EventHash == nftPurchaseEventHash && x.NotificationType == notificationType)
            .AnyAsync(stoppingToken);
}
