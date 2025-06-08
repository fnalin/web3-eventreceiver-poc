using Fansoft.Blockchain.Scout.Api.Models;
using Fansoft.Blockchain.Scout.Api.Services;
using Microsoft.Extensions.Options;
using Nethereum.Web3;

namespace Fansoft.Blockchain.Scout.Api.Blockchain.Listeners;

public class BlockListener: BackgroundService
{
    private readonly ILogger<BlockListener> _logger;
    private readonly IBlockRepository _repository;
    private readonly Web3 _web3;

    private ulong _lastBlockNumber = 0;

    public BlockListener(ILogger<BlockListener> logger, IBlockRepository repository, IOptions<BlockchainOptions> settings)
    {
        _logger = logger;
        _repository = repository;
        _web3 = new Web3(settings.Value.RpcUrl);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Block listener iniciado.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                 var latestBlockNumber = (ulong)(await _web3.Eth.Blocks.GetBlockNumber.SendRequestAsync()).Value;
                _lastBlockNumber = (ulong)(await _repository.GetMaxBlockNumberAsync());
                 _logger.LogInformation("Último bloco indexado: {Block}", _lastBlockNumber);
                for (ulong i = _lastBlockNumber + 1; i <= latestBlockNumber; i++)
                {
                    if (await _repository.ExistsAsync((long)i)) continue;
                    _logger.LogInformation("Indexando bloco {BlockNumber}...", i);
                    var block = await _web3.Eth.Blocks.GetBlockWithTransactionsByNumber
                        .SendRequestAsync(new Nethereum.RPC.Eth.DTOs.BlockParameter(i));

                    if (block is null) continue;

                    var model = new BlockModel
                    {
                        Hash = block.BlockHash,
                        Number = (long)block.Number.Value,
                        Timestamp = DateTimeOffset.FromUnixTimeSeconds((long)block.Timestamp.Value).UtcDateTime,
                        Transactions = block.Transactions.Select(tx => tx.TransactionHash).ToList()
                    };

                    await _repository.SaveBlockAsync(model);

                    _logger.LogInformation("Bloco {BlockNumber} indexado.", model.Number);

                    _lastBlockNumber = (ulong)model.Number;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao monitorar blocos.");
            }

            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }
    }
}