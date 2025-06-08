using Fansoft.EventReceiver.Api.Data.Repositories;
using Fansoft.EventReceiver.Api.Models;
using Fansoft.EventReceiver.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Fansoft.EventReceiver.Api.Endpoints;

public static class NftsEndpoints
{
    public static void MapNftsEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/v1/nfts")
            .WithTags("NFT Marketplace");
        
        group.MapPost("/purchase", async (
            [FromBody] PurchaseAccessDto dto,
            INftService nftService,
            ILoggerFactory loggerFactory
        ) =>
        {
            var logger = loggerFactory.CreateLogger("Purchase");
    
            try
            {
                await nftService.GrantAccessAsync(dto);
                return Results.Ok(new { success = true });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erro ao conceder acesso ao token {TokenId}", dto.TokenId);
                return Results.Problem(title: "Erro ao processar compra", detail: ex.Message);
            }
        });
        
        group.MapGet("/wallets/{tokenId:long}", async (
            long tokenId,
            INftPurchaseRepository purchaseRepo) =>
        {
            var wallets = await purchaseRepo.GetWalletsByTokenIdAsync(tokenId);
            return Results.Ok(new { tokenId, wallets });
        });
        
    }
}