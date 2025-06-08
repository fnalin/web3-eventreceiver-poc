using Fansoft.EventReceiver.Api.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Fansoft.EventReceiver.Api.Data.Repositories;

public class NftPurchaseRepository : INftPurchaseRepository
{
    private readonly IMongoCollection<NftPurchase> _collection;
    private readonly ILogger<NftPurchaseRepository> _logger;

    public NftPurchaseRepository(IOptions<MongoDbOptions> options, ILogger<NftPurchaseRepository> logger)
    {
        _logger = logger;
        if (options == null || string.IsNullOrEmpty(options.Value.ConnectionString) || string.IsNullOrEmpty(options.Value.Database))
        {
            throw new ArgumentException("MongoDB options are not properly configured.");
        }
        var client = new MongoClient(options.Value.ConnectionString);
        var database = client.GetDatabase(options.Value.Database);
        _collection = database.GetCollection<NftPurchase>("nftPurchases");
        EnsureIndexesAsync().GetAwaiter().GetResult();
    }
    
    public async Task<NftPurchase?> GetByTokenIdAsync(long tokenId) =>
        await _collection.Find(x => x.TokenId == tokenId).FirstOrDefaultAsync();
       

    public async Task AddWalletToPurchaseAsync(long tokenId, string eventHash, string wallet)
    {
        var filter = Builders<NftPurchase>.Filter.Eq(x => x.TokenId, tokenId);
        var update = Builders<NftPurchase>.Update
            .SetOnInsert(x => x.EventHash, eventHash)
            .AddToSet(x => x.Wallets, wallet);

        await _collection.UpdateOneAsync(filter, update, new UpdateOptions { IsUpsert = true });
    }

    public async Task<List<string>> GetWalletsByTokenIdAsync(long tokenId)
    {
        var result = await GetByTokenIdAsync(tokenId);
        return result?.Wallets ?? new List<string>();
    }
    
    private async Task EnsureIndexesAsync()
    {
        var indexKeys = Builders<NftPurchase>.IndexKeys.Ascending(x => x.TokenId);
        var indexOptions = new CreateIndexOptions { Unique = true };

        var indexModel = new CreateIndexModel<NftPurchase>(indexKeys, indexOptions);
        await _collection.Indexes.CreateOneAsync(indexModel);
    }
}

public interface INftPurchaseRepository
{
    Task<NftPurchase?> GetByTokenIdAsync(long tokenId);
    Task AddWalletToPurchaseAsync(long tokenId, string eventHash, string wallet);
    Task<List<string>> GetWalletsByTokenIdAsync(long tokenId);
}