using System.Numerics;

namespace Fansoft.EventReceiver.Api.Models;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class EventDocument
{
    [BsonId]
    public ObjectId Id { get; set; }

    public string EventHash { get; set; } = null!;

    public DigitalTwinField DigitalTwin { get; set; } = null!;
    
    public List<EventField> Fields { get; set; } = [];

    public DateTime ReceivedAt { get; set; } = DateTime.UtcNow;
    public string TokenUri { get; set; } = null!;
    public long TokenId { get; set; } 
}

public class EventField
{
    public string Name { get; set; } = null!;
    public string Type { get; set; } = null!;
    public object? Value { get; set; }
}

public class DigitalTwinField
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Address { get; set; } = null!;
    public string CallBackUrl { get; set; } = null!;
}

public class MongoDbOptions
{
    public string ConnectionString { get; set; } = null!;
    public string Database { get; set; } = null!;
}

public class NftPurchase
{
    [BsonId]
    public ObjectId Id { get; set; }

    [BsonElement("tokenId")]
    public long TokenId { get; set; }

    [BsonElement("eventHash")]
    public string EventHash { get; set; } = null!;

    [BsonElement("wallets")]
    public List<string> Wallets { get; set; } = [];
}

public class PurchaseNotification
{
    [BsonId]
    public ObjectId Id { get; set; }
    
    [BsonElement("eventHash")]
    public string EventHash { get; set; } = null!;
    
    [BsonElement("wallet")]
    public string Wallet { get; set; } = null!;

    [BsonElement("appProviderId")]
    public int AppProviderId { get; set; }
    
    [BsonRepresentation(BsonType.String)]
    [BsonElement("notificationType")]
    public NotificationType NotificationType { get; set; }

    [BsonElement("notificationConfirm")]
    public bool NotificationConfirm { get; set; } = true;
    
    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
}

public enum NotificationType
{
    RabbitMq = 1,
    Webhook = 2
}