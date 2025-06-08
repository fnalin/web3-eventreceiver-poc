namespace Fansoft.AppProvider.Api.Data.Entities;

public class EventProcess
{
    public int Id { get; set; }
    public StatusProcess Status { get; set; } = StatusProcess.Pending;
    public string EventHash { get; set; } = null!;
    public string? OriginalPayload { get; set; }
    public string? FailureReason { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ProcessedAt { get; set; }
}

public enum StatusProcess
{
    Pending = 0,
    Completed = 1,
    Failed = 2
}