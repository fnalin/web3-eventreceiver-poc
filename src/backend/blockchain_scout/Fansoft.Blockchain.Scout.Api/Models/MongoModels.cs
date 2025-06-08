namespace Fansoft.Blockchain.Scout.Api.Models;

public class MongoDbOptions
{
    public string ConnectionString { get; set; } = null!;
    public string Database { get; set; } = null!;
}