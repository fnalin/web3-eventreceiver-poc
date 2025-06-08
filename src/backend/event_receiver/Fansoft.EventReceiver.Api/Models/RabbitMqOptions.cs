namespace Fansoft.EventReceiver.Api.Models;

public class RabbitMqOptions
{
    public string ConnectionString { get; set; } = null!;
    public string Prefix { get; set; } = null!;
}