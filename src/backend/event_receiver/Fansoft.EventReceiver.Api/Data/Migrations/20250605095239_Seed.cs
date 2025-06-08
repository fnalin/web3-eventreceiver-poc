using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fansoft.EventReceiver.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class Seed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($@"
                INSERT INTO DigitalTwins (KeyName, WalletId, DtCallbackUrl) VALUES 
                ('DT1', '0x70997970C51812dc3A010C7d01b50e0d17dc79C8', 'https://localhost:5006/api/v1/events'),
                ('DT2', '0x3C44CdDdB6a900fa2b585dd299e03d12FA4293BC', 'http://localhost:5005/api/v1/events');
            ");

            migrationBuilder.Sql($@"
                INSERT INTO ApplicationProviders
                    (KeyName, WalletId, AppWebHookUrl, AmqpHost, AmqpPort, AmqpVirtualHost, AmqpUser, AmqpPassword, AmqpQueueName, EnableWebhook, EnableAmqp, IsActive, CreatedAt)
                VALUES
                    ('app-provider-1', '0x90F79bf6EB2c4f870365E785982E1f101E93b906', 'https://localhost:5008/api/v1/webhooks/nfts', 'localhost', 5672, 'broker.app1', 'app1_user', 'app123Pass', 'app1.queue.nft', 1, 1, 1, CURRENT_TIMESTAMP),
                    ('app-provider-2', '0x15d34aaf54267db7d7c367839aaf71a00a2c6a65', NULL, 'localhost', 5672, 'broker.app2', 'app2_user', 'app123Pass', 'app2.queue.nft', 0, 1, 0, CURRENT_TIMESTAMP)
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
