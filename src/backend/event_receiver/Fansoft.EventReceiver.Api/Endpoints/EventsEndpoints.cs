using System.Text.Json;
using Fansoft.EventReceiver.Api.Data.Repositories;
using Fansoft.EventReceiver.Api.Models;
using Fansoft.EventReceiver.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Fansoft.EventReceiver.Api.Endpoints;

public static class EventsEndpoints
{
    public static void MapEventEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/v1/events")
            .WithTags("Events");
            //.RequireAuthorization();

        group.MapPost("/", async (
                [FromBody] JsonElement payload,
                [FromQuery] string twinName,
                INftService nftService,
                IDigitalTwinRepository digitalTwinRepository,
                ILogger<Program> logger) =>
            {
                var dt = await digitalTwinRepository.GetAddressByNameAsync(twinName);
               
                try
                {
                    var mintDto = new MintEventDto(dt, payload);
                    var dataEvent = await nftService.MintOriginalAsync(mintDto);
                    
                    return Results.Ok(new
                    {
                        success = true,
                        dataEvent
                    });
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Erro ao mintar NFT");
                    return Results.BadRequest(new
                    {
                        success = false,
                        error = ex.Message
                    });
                }
            }
        );
        
        group.MapGet("/{eventHash}", async (string eventHash, IEventRepository repo) =>
        {
            var evento = await repo.GetByEventHashAsync(eventHash);
            
            var metadata = new {
                name = $"Evento #{eventHash}",
                description = "Token gerado a partir de evento digital twin",
                tokenUri = $"{evento?.TokenUri}",
                tokenId = evento?.TokenId,
                DT = evento?.DigitalTwin?.Name,
                WalletOwner = evento?.DigitalTwin?.Address,
                //image = "https://example.com/static/event.jpg", // opcional
                attributes = evento?.Fields.Select(f => new {
                    f.Name, f.Type
                })
            };

            return Results.Json(metadata);
        });
        
        group.MapGet("/", async (IEventRepository repo, CancellationToken cancellation) =>
        {
            var events = await repo.GetAllAsync(cancellation);
            var metadata = events.Select(e => new
            {
                eventHash = e.EventHash,
                description = $"Token gerado a partir de evento digital twin {e?.DigitalTwin?.Name}",
                tokenUri = $"{e?.TokenUri}",
                tokenId = e?.TokenId,
                DT = e?.DigitalTwin?.Name,
                WalletOwner = e?.DigitalTwin?.Address,
                attributes = e?.Fields.Select(f => new
                {
                    f.Name, f.Type
                })
            });

            return Results.Json(metadata);
        });
        
        group.MapGet("/{eventHash}/download", async (
            string eventHash,
            [FromQuery] string wallet,
            IEventRepository eventRepo,
            INftPurchaseRepository purchaseRepo) =>
        {
            if (!Nethereum.Util.AddressUtil.Current.IsValidEthereumAddressHexFormat(wallet))
                return Results.BadRequest("Endereço de carteira inválido.");

            var evt = await eventRepo.GetByEventHashAsync(eventHash);
            if (evt == null)
                return Results.NotFound("Evento não encontrado.");

            var wallets = await purchaseRepo.GetWalletsByTokenIdAsync(evt.TokenId);
            if (!wallets.Contains(wallet, StringComparer.OrdinalIgnoreCase))
                return Results.StatusCode(403);

            // Simula o conteúdo do evento a ser baixado
            var fakeJson = new
            {
                name = $"Evento {eventHash}",
                twin = evt.DigitalTwin?.Name,
                payload = evt.Fields.ToDictionary(f => f.Name, f => f.Value),
                timestamp = evt.ReceivedAt,
            };

            return Results.Json(fakeJson);
        });
    }
}