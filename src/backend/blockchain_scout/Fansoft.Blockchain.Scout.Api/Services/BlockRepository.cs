using Fansoft.Blockchain.Scout.Api.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Fansoft.Blockchain.Scout.Api.Services;

public interface IBlockRepository
{
    Task SaveBlockAsync(BlockModel block);
    Task<BlockModel?> GetByHashAsync(string hash);
    Task<bool> ExistsAsync(long blockNumber);
    Task<long> GetMaxBlockNumberAsync();
    
    Task<List<BlockModel>> GetPagedAsync(int page, int pageSize);
    Task<long> CountAsync();
}

public class BlockRepository : IBlockRepository
{
    private readonly IMongoCollection<BlockModel> _collection;

    public BlockRepository(IOptions<MongoDbOptions> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        var db = client.GetDatabase(settings.Value.Database);
        _collection = db.GetCollection<BlockModel>("blocks");
    }

    public Task SaveBlockAsync(BlockModel block) =>
        _collection.InsertOneAsync(block);

    public async Task<BlockModel?> GetByHashAsync(string hash) =>
        await _collection.Find(b => b.Hash == hash).FirstOrDefaultAsync();
    
    public async Task<bool> ExistsAsync(long blockNumber)
    {
        var filter = Builders<BlockModel>.Filter.Eq(b => b.Number, blockNumber);
        return await _collection.Find(filter).AnyAsync();
    }
    
    public async Task<long> GetMaxBlockNumberAsync()
    {
        var result = await _collection.Find(FilterDefinition<BlockModel>.Empty)
            .Project(b => new { b.Number })
            .SortByDescending(b => b.Number)
            .Limit(1)
            .FirstOrDefaultAsync();

        return result?.Number ?? 0;
    }
    
    public async Task<List<BlockModel>> GetPagedAsync(int page, int pageSize)
    {
        return await _collection.Find(Builders<BlockModel>.Filter.Empty)
            .SortByDescending(b => b.Number)
            .Skip((page - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync();
    }

    public async Task<long> CountAsync()
    {
        return await _collection.CountDocumentsAsync(Builders<BlockModel>.Filter.Empty);
    }
}