using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using System.Text;
using System.Security.Cryptography;
using System.Text.Json;
using Fansoft.EventReceiver.Api.Data.Repositories;
using Fansoft.EventReceiver.Api.Models;
using Microsoft.Extensions.Options;
using Nethereum.Contracts;

namespace Fansoft.EventReceiver.Api.Services;

public class NftService : INftService
{
    private readonly IEventRepository _eventRepository;
    private readonly ILogger<NftService> _logger;
    private readonly BlockchainOptions _options;
    private readonly Account _account;
    private readonly Web3 _web3;
    private readonly Contract _contract;
    private readonly INftPurchaseRepository _nftPurchaseRepository;


    public NftService(IEventRepository eventRepository, INftPurchaseRepository nftPurchaseRepository,
        ILogger<NftService> logger, IOptions<BlockchainOptions> options)
    {
        _eventRepository = eventRepository ?? throw new ArgumentNullException(nameof(eventRepository));
        _nftPurchaseRepository = nftPurchaseRepository ?? throw new ArgumentNullException(nameof(nftPurchaseRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _options = (options ?? throw new ArgumentNullException(nameof(options))).Value;

        _account = new Account(options.Value.PrivateKey);
        _web3 = new Web3(_account, options.Value.RpcUrl);
        var abi = LoadContractAbi();
        _contract = _web3.Eth.GetContract(abi, options.Value.ContractAddress);
    }

    private string LoadContractAbi()
    {
        return File.ReadAllText(_options.AbiPath);
    }

    


    public async Task<EventResponse> MintOriginalAsync(MintEventDto mintEventDto)
    {
       
        var stringContent = mintEventDto.Je.GetRawText();
        var toAddress = mintEventDto.Dt.WalletId;
        
        var composite = $"{toAddress}|{stringContent}";
        var eventHash = ComputeEventHash(composite);

        var isRegisteredFn = _contract.GetFunction("isEventRegistered");
        var already = await isRegisteredFn.CallAsync<bool>(eventHash);
        if (already)
        {
            _logger.LogWarning("Event já registrado: {Hash}", eventHash);
            throw new InvalidOperationException("Event already tokenized");
        }

        // 🔹 Monta o tokenUri com base no eventHash
        var tokenUriToMint = _options.TokenIdUri + ToHexString(eventHash);

        _logger.LogInformation("Mintando NFT para {To} com URI {Uri}", toAddress, tokenUriToMint);
        
        var mintFor = _contract.GetFunction("mintFor");
        var receipt = await mintFor.SendTransactionAndWaitForReceiptAsync(
            from: _account.Address,
            gas: new Nethereum.Hex.HexTypes.HexBigInteger(300_000),
            value: null,
            functionInput: [toAddress, tokenUriToMint, eventHash]
        );

        _logger.LogInformation("Transação realizada: {Tx}", receipt.TransactionHash);
        
        var transferEventHandler = _web3.Eth.GetEvent<NftTransferEventDTO>(_options.ContractAddress);
        var decodedEvents = transferEventHandler.DecodeAllEventsForEvent(receipt.Logs);
        var transferEvent = decodedEvents.FirstOrDefault()?.Event;
        
        if (transferEvent == null)
        {
            _logger.LogWarning("Transfer event not found in transaction logs.");
            throw new Exception("Token mint failed: no Transfer event.");
        }
        
        var eventDoc = new EventDocument
        {
            EventHash = receipt.TransactionHash,
            DigitalTwin = new DigitalTwinField() { 
                Id = mintEventDto.Dt.Id,
                Name = mintEventDto.Dt.KeyName,
                Address = mintEventDto.Dt.WalletId,
                CallBackUrl = mintEventDto.Dt.DtCallbackUrl
            },
            Fields = EventFieldExtractor.ExtractFields(mintEventDto.Je), // função que mapeia o JSON para os campos
            TokenUri = tokenUriToMint,
            TokenId = (long)transferEvent.TokenId,
        };

        await _eventRepository.SaveAsync(eventDoc);

        return new EventResponse(receipt.TransactionHash);

    }
    
    public async Task GrantAccessAsync(PurchaseAccessDto dto)
    {
        
        if (!Nethereum.Util.AddressUtil.Current.IsValidEthereumAddressHexFormat(dto.BuyerAddress))
            throw new ArgumentException("Endereço do comprador inválido.");
        
        var eventOriginal = await _eventRepository.GetByTokenIdAsync(dto.TokenId);
        if (eventOriginal == null)
        {
            _logger.LogWarning("TokenId {TokenId} not fount in Events collection", dto.TokenId);
            throw new InvalidOperationException("TokenId not found");
        }
        
        var existingWallets = await _nftPurchaseRepository.GetWalletsByTokenIdAsync(dto.TokenId);
        if (existingWallets.Contains(dto.BuyerAddress, StringComparer.OrdinalIgnoreCase))
        {
            _logger.LogWarning("Wallet {BuyerAddress} já possui acesso ao token {TokenId}", dto.BuyerAddress, dto.TokenId);
            throw new InvalidOperationException("Esta carteira já comprou acesso a este NFT.");
        }
        
        var grantAccessFn = _contract.GetFunction("grantAccess");

        var receipt = await grantAccessFn.SendTransactionAndWaitForReceiptAsync(
            from: _account.Address,
            gas: new Nethereum.Hex.HexTypes.HexBigInteger(200_000),
            value: null,
            functionInput: [dto.TokenId, dto.BuyerAddress]
        );

        
        
        await _nftPurchaseRepository.AddWalletToPurchaseAsync(dto.TokenId, eventOriginal.EventHash, dto.BuyerAddress);
        
        _logger.LogInformation("Acesso concedido ao token {TokenId} para {BuyerAddress}. Tx: {Tx}",
            dto.TokenId, dto.BuyerAddress, receipt.TransactionHash);
    }

    private static byte[] ComputeEventHash(string input)
    {
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(input));
    
        if (hash.Length != 32)
            throw new Exception("SHA256 deve retornar exatamente 32 bytes.");

        return hash;
    }

    private static string ToHexString(byte[] bytes)
    {
        return "0x" + BitConverter.ToString(bytes).Replace("-", "").ToLowerInvariant();
    }
   
}

public interface INftService
{
    Task<EventResponse> MintOriginalAsync(MintEventDto mintEventDto);
    Task GrantAccessAsync(PurchaseAccessDto dto);
}

public static class EventFieldExtractor
{
    public static List<EventField> ExtractFields(JsonElement payload)
    {
        var fields = new List<EventField>();

        foreach (var property in payload.EnumerateObject())
        {
            var name = property.Name;
            var value = property.Value;

            var (type, typedValue) = GetValueAndType(value);

            fields.Add(new EventField
            {
                Name = name,
                Type = type,
                Value = typedValue
            });
        }

        return fields;
    }

    private static (string Type, object? Value) GetValueAndType(JsonElement value, string name = "")
    {
        if (value.ValueKind == JsonValueKind.String)
        {
            var stringValue = value.GetString();

            // Tenta converter para DateTime
            if (DateTime.TryParse(stringValue, out var dt))
                return ("datetime", dt);

            // Tenta converter para coordenadas geográficas
            if (name.ToLower() is "latitude" or "lat" && double.TryParse(stringValue, out var lat))
                return ("latitude", lat);

            if (name.ToLower() is "longitude" or "lng" && double.TryParse(stringValue, out var lng))
                return ("longitude", lng);

            return ("string", stringValue);
        }

        return value.ValueKind switch
        {
            JsonValueKind.Number => value.TryGetInt64(out var longVal)
                ? ("int64", longVal)
                : ("double", value.GetDouble()),

            JsonValueKind.True or JsonValueKind.False => ("bool", value.GetBoolean()),
            JsonValueKind.Null => ("null", null),
            JsonValueKind.Object => ("object", value.ToString()),
            JsonValueKind.Array => ("array", value.ToString()),
            _ => ("unknown", value.ToString())
        };
    }
}