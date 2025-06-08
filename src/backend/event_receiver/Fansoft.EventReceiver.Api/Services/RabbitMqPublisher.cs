using System.Text;
using Fansoft.EventReceiver.Api.Models;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace Fansoft.EventReceiver.Api.Services;

public interface IRabbitMqPublisher
{
    Task PublishEventHashAsync(string eventHash, string vHost, string queueName, CancellationToken cancellationToken = default);
}

public class RabbitMqPublisher (IOptions<RabbitMqOptions> options) : IRabbitMqPublisher, IAsyncDisposable
{
    public async Task PublishEventHashAsync(string eventHash, string vHost, string queueName, 
        CancellationToken cancellationToken = default)
    {
        var  factory = new ConnectionFactory
        {
            Uri = new Uri(options.Value.ConnectionString + $"/{vHost}")
        };
        
        await using var connection = await factory.CreateConnectionAsync(cancellationToken);
        await using var channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);

        await channel.QueueDeclareAsync(
            queue: queueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null,
            cancellationToken: cancellationToken);

        var body = Encoding.UTF8.GetBytes(eventHash);

        await channel.BasicPublishAsync(
            exchange: "",
            routingKey: queueName,
            body: body,
            cancellationToken: cancellationToken);
    }

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;
}