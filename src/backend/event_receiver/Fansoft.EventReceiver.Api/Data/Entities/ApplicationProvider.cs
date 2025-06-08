namespace Fansoft.EventReceiver.Api.Data.Entities;

public class ApplicationProvider
{
    public int Id { get; set; }

    // Nome da integração ou identificador público (ex: "hospital-santa-maria")
    public string KeyName { get; set; } = null!;

    // Wallet do comprador (quem terá acesso ao conteúdo NFT)
    public string WalletId { get; set; } = null!;

    // Endereço para notificação via HTTP
    public string? AppWebHookUrl { get; set; }

    // Configuração de conexão AMQP (RabbitMQ)
    public string? AmqpHost { get; set; }      // ex: "rabbit.partner.com"
    public int? AmqpPort { get; set; }         // ex: 5672
    public string? AmqpVirtualHost { get; set; } = "/"; // vhost personalizado
    public string? AmqpUser { get; set; }
    public string? AmqpPassword { get; set; }

    // Fila que ele espera consumir
    public string? AmqpQueueName { get; set; }

    // Controle de notificações ativas
    public bool EnableWebhook { get; set; } = true;
    public bool EnableAmqp { get; set; } = false;

    // Auditoria
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}