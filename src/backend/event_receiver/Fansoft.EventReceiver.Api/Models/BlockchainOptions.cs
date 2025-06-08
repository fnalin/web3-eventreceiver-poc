namespace Fansoft.EventReceiver.Api.Models;

public class BlockchainOptions
{
    public string PrivateKey { get; set; } = null!;
    public string RpcUrl { get; set; } = null!;
    public string ContractAddress { get; set; } = null!;
    public string AbiPath { get; set; } = null!;
    public string TokenIdUri { get; set; } = null!;
}