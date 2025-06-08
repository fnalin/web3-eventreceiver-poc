namespace Fansoft.EventReceiver.Api.Models;

using Nethereum.ABI.FunctionEncoding.Attributes;
using System.Numerics;

[Event("Transfer")]
public class NftTransferEventDTO : IEventDTO
{
    [Parameter("address", "from", 1, true)]
    public string From { get; set; } = default!;

    [Parameter("address", "to", 2, true)]
    public string To { get; set; } = default!;

    [Parameter("uint256", "tokenId", 3, true)]
    public BigInteger TokenId { get; set; }
}

[Event("GrantedAccessNFT")]
public class AccessGrantedEventDTO : IEventDTO
{
    [Parameter("uint256", "tokenId", 1, true)]
    public BigInteger TokenId { get; set; }

    [Parameter("address", "buyer", 2, true)]
    public string Buyer { get; set; }
}