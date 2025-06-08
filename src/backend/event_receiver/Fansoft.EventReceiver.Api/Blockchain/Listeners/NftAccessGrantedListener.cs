using Fansoft.EventReceiver.Api.Data.Repositories;
using Fansoft.EventReceiver.Api.Models;
using Fansoft.EventReceiver.Api.Services;
using Microsoft.Extensions.Options;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;

namespace Fansoft.EventReceiver.Api.Blockchain.Listeners;

public class NftAccessGrantedListener : BackgroundService
{
    private readonly ILogger<NftAccessGrantedListener> _logger;
    private readonly Web3 _web3;
    //private readonly HttpClient _httpClient;
    private readonly string _contractAddress;
    private readonly IRabbitMqPublisher _rabbitMqPublisher;
    private readonly IServiceProvider _serviceProvider;

    public NftAccessGrantedListener(ILogger<NftAccessGrantedListener> logger,
        // IHttpClientFactory httpClientFactory, 
        IRabbitMqPublisher rabbitMqPublisher,
        IServiceProvider serviceProvider,
        IOptions<BlockchainOptions> options)
    {
        _logger = logger;
        var account = new Account(options.Value.PrivateKey);
        _web3 = new Web3(account, options.Value.RpcUrl);
        _rabbitMqPublisher = rabbitMqPublisher ?? throw new ArgumentNullException(nameof(rabbitMqPublisher));
        //_httpClient = httpClientFactory.CreateClient("NotifyClient");
        _serviceProvider = serviceProvider;
        _contractAddress = options.Value.ContractAddress;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var handler = _web3.Eth.GetEvent<AccessGrantedEventDTO>(_contractAddress);
        //var filterAll = await handler.CreateFilterAsync();
        var latestBlock = await _web3.Eth.Blocks.GetBlockNumber.SendRequestAsync();

        var filterInput = handler.CreateFilterInput(
            fromBlock: new BlockParameter(latestBlock),
            toBlock: BlockParameter.CreateLatest()
        );

        var filterAll = await handler.CreateFilterAsync(filterInput);

        while (!stoppingToken.IsCancellationRequested)
        {
            var logs = await handler.GetFilterChangesAsync(filterAll);
            
            using var scope = _serviceProvider.CreateScope();
            var nftRepo = scope.ServiceProvider.GetRequiredService<INftPurchaseRepository>();
            var purchaseNotRepo = scope.ServiceProvider.GetRequiredService<IPurchaseNotificationRepository>();
            var appRepo = scope.ServiceProvider.GetRequiredService<IApplicationProviderRepository>();
            var http = scope.ServiceProvider.GetRequiredService<IHttpClientFactory>().CreateClient();

            foreach (var log in logs)
            {
                var tokenId = log.Event.TokenId;
                var wallet = log.Event.Buyer;

                var nftPurchase = await nftRepo.GetByTokenIdAsync((long)tokenId);
                _logger.LogInformation("Acesso concedido - TokenId: {TokenId}, Wallet: {Wallet}, EventHash: {EventHash}", 
                    tokenId, wallet, nftPurchase?.EventHash ?? "N/A");

                if (nftPurchase?.EventHash == null) continue;
                
                var appProv = await appRepo.GetByWalletAsync(wallet);
                if (appProv == null)
                {
                    _logger.LogWarning("Nenhum ApplicationProvider encontrado para a Wallet {Wallet}", wallet);
                    continue;
                }
                var isSentToPurchaseNotificationAmqp = await purchaseNotRepo.IsSentNotificationAsync(nftPurchase.EventHash, NotificationType.RabbitMq, stoppingToken);
                if (isSentToPurchaseNotificationAmqp)
                {
                    _logger.LogInformation("Notificação RabbitMQ já enviada para o ApplicationProvider {AppProvider} - EventHash: {EventHash}", appProv.KeyName, nftPurchase.EventHash);
                }
                
                if (appProv is { EnableAmqp: true, IsActive: true } && !isSentToPurchaseNotificationAmqp)
                {
                    await _rabbitMqPublisher.PublishEventHashAsync(nftPurchase.EventHash, appProv.AmqpVirtualHost!,
                        appProv.AmqpQueueName!, stoppingToken);

                    _logger.LogInformation(
                        "Notificação RabbitMQ enviada para o ApplicationProvider {AppProvider} - EventHash: {EventHash}", appProv.KeyName, nftPurchase.EventHash);
                    
                    await purchaseNotRepo.AddNotificationAsync(new PurchaseNotification()
                    {
                        EventHash = nftPurchase.EventHash,
                        Wallet = wallet,
                        AppProviderId = appProv.Id,
                        NotificationType = NotificationType.RabbitMq
                    }, stoppingToken);
                }

                var isSentToPurchaseNotificationWebHook = await purchaseNotRepo.IsSentNotificationAsync(nftPurchase.EventHash, NotificationType.Webhook, stoppingToken);
                if (isSentToPurchaseNotificationWebHook)
                {
                    _logger.LogInformation("Notificação WebHook já enviada para o ApplicationProvider {AppProvider} - EventHash: {EventHash}", appProv.KeyName, nftPurchase.EventHash);
                }
                if (appProv is { EnableWebhook: true, IsActive: true } && !string.IsNullOrEmpty(appProv.AppWebHookUrl) && !isSentToPurchaseNotificationWebHook)
                {
                    var notificated = new PurchaseNotification()
                    {
                        EventHash = nftPurchase.EventHash,
                        Wallet = wallet,
                        AppProviderId = appProv.Id,
                        NotificationType = NotificationType.Webhook
                    };
                    
                    // Notifica um serviço externo (ex: ApplicationProvider)
                    try
                    {
                        var response = await http.PostAsJsonAsync(appProv.AppWebHookUrl, new
                        {
                            nftPurchase.EventHash
                        }, cancellationToken: stoppingToken);

                        if (!response.IsSuccessStatusCode)
                        {
                            _logger.LogWarning("Falha ao notificar acesso de TokenId {TokenId} para Wallet {Wallet} - StatusCode: {StatusCode}", 
                                tokenId, wallet, response.StatusCode);
                            notificated.NotificationConfirm = false;
                        }
                        else
                        {
                            _logger.LogInformation("Acesso de TokenId {TokenId} notificado com sucesso para Wallet {Wallet} - via webhook - StatusCode: {StatusCode}", 
                                tokenId, wallet, response.StatusCode);
                            notificated.NotificationConfirm = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Erro ao enviar webhook para ApplicationProvider {AppProvider} - Wallet: {Wallet}", appProv.KeyName, wallet);
                        notificated.NotificationConfirm = false;
                    }
                    
                    await purchaseNotRepo.AddNotificationAsync(notificated, stoppingToken);
                }
                
               
            }

            await Task.Delay(5000, stoppingToken);
        }
    }
}