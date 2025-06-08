using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Fansoft.Blockchain.Scout.Api.Models;


public class BlockModel
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    public string Hash { get; set; } = null!;
    public long Number { get; set; }
    public DateTime Timestamp { get; set; }
    public List<string> Transactions { get; set; } = [];
}

public class BlockchainOptions
{
    public string RpcUrl { get; set; } = null!;
}

public class PagedResult<T>
{
    public List<T> Items { get; set; } = [];
    public int Page { get; set; }
    public int PageSize { get; set; }
    public long TotalItems { get; set; }
}

public class BlockQueryParams
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}