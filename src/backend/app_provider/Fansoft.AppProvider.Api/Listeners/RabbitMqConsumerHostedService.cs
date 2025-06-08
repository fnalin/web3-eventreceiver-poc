using System.Text;
using Fansoft.AppProvider.Api.Data.Entities;
using Fansoft.AppProvider.Api.Data.Repositories;
using Fansoft.AppProvider.Api.Models;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Fansoft.AppProvider.Api.Listeners;

public class RabbitMqConsumerHostedService (
    IOptions<RabbitMqOptions> options, IServiceProvider serviceProvider, ILogger<RabbitMqConsumerHostedService> log) : BackgroundService
{
    private IConnection? _connection;
    private IChannel? _channel;
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory
        {
            Uri = new Uri(options.Value.ConnectionString)
        };

        _connection = await factory.CreateConnectionAsync(stoppingToken);
        _channel = await _connection.CreateChannelAsync(cancellationToken: stoppingToken);

        var queue = options.Value.QueueName;
        
        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += async (sender, eventArgs) =>
        {
            try
            {
                var body = eventArgs.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                log.LogInformation("[x] Received {Message}", message);

                // Save event
                using var scope = serviceProvider.CreateScope();
                var eventProcessRepo = scope.ServiceProvider.GetRequiredService<IEventProcessRepository>();
                await eventProcessRepo.AddEventProcess(new EventProcess()
                {
                    EventHash = message
                });
                
                // await _channel.BasicAckAsync(deliveryTag: eventArgs.DeliveryTag, multiple: false,
                //     cancellationToken: stoppingToken);

                await ((AsyncEventingBasicConsumer)(sender)).Channel.BasicAckAsync(eventArgs.DeliveryTag,
                    multiple: false, cancellationToken: stoppingToken);

            } catch (Exception ex)
            {
                log.LogError("Erro ao processar mensagem: {ExMessage}", ex.Message);

                // Opcional: rejeita e reencaminha para dead-letter, se configurado
                await _channel.BasicNackAsync(eventArgs.DeliveryTag, multiple: false, requeue: false, cancellationToken: stoppingToken);
            }
        };
        
        await _channel.BasicConsumeAsync(queue, autoAck: false, consumer: consumer, cancellationToken: stoppingToken);
    }
}