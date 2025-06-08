using Fansoft.EventReceiver.Api.Models;
using Microsoft.Extensions.Options;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;

namespace Fansoft.EventReceiver.Api.Blockchain.Listeners;

public class NftCreatedListener : BackgroundService
{
    private readonly ILogger<NftCreatedListener> _logger;
    private readonly Web3 _web3;
    private readonly string _contractAddress;

    public NftCreatedListener(ILogger<NftCreatedListener> logger, IOptions<BlockchainOptions> options)
    {
        _logger = logger;
        var account = new Account(options.Value.PrivateKey);
        _web3 = new Web3(account, options.Value.RpcUrl);
        _contractAddress = options.Value.ContractAddress;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var handler = _web3.Eth.GetEvent<NftTransferEventDTO>(_contractAddress);

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

            foreach (var log in logs)
            {
                _logger.LogInformation("NFT criado - TokenId: {TokenId}, De: {From}, Para: {To}",
                    log.Event.TokenId, log.Event.From, log.Event.To);
            }

            await Task.Delay(5000, stoppingToken);
        }
    }
}