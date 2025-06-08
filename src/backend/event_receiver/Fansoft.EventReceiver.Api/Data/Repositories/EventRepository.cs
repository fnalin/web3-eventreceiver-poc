using Fansoft.EventReceiver.Api.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Fansoft.EventReceiver.Api.Data.Repositories;

public interface IEventRepository
{
    Task SaveAsync(EventDocument doc);
    Task<EventDocument> GetByEventHashAsync(string hash);
    Task<EventDocument> GetByTokenIdAsync(long tokenId);
    Task<IEnumerable<EventDocument>> GetAllAsync(CancellationToken cancellation = default);

}

public class EventRepository : IEventRepository
{
    private readonly IMongoCollection<EventDocument> _collection;

    public EventRepository(IOptions<MongoDbOptions> options)
    {
        var client = new MongoClient(options.Value.ConnectionString);
        var database = client.GetDatabase(options.Value.Database);
        _collection = database.GetCollection<EventDocument>("events");
    }

    public Task SaveAsync(EventDocument doc) =>
        _collection.InsertOneAsync(doc);
    
    public Task<EventDocument> GetByEventHashAsync(string eventHash) => 
        _collection.Find(x => x.EventHash == eventHash).FirstOrDefaultAsync();

    public async Task<EventDocument> GetByTokenIdAsync(long tokenId) =>
        await _collection.Find(x => x.TokenId == tokenId).FirstOrDefaultAsync();

    public async Task<IEnumerable<EventDocument>> GetAllAsync(CancellationToken cancellation = default) =>
        await _collection
            .Find(_ => true)
            .SortByDescending(e => e.ReceivedAt)
            .ToListAsync(cancellation);
}