namespace Fansoft.DigitalTwin.Api.Data.Entities;

public class StoredEvent
{
    public int Id { get; set; }
    public string ExternalId { get; set; } = null!;
    public string OriginalPayload { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}