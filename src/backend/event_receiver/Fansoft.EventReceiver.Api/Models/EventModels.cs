using System.Text.Json;
using Fansoft.EventReceiver.Api.Data.Entities;

namespace Fansoft.EventReceiver.Api.Models;

public record EventResponse(string TransactionHash);

public record MintEventDto(DigitalTwin Dt,JsonElement Je);

public record PurchaseAccessDto(long TokenId, string BuyerAddress);