namespace Fansoft.AppProvider.Api.Models;

public class RabbitMqOptions
{
    public string ConnectionString { get; set; } = null!;
    public string QueueName { get; set; } = null!;
}