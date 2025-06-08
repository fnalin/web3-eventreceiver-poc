using Fansoft.Blockchain.Scout.Api.Models;
using Fansoft.Blockchain.Scout.Api.Services;

namespace Fansoft.Blockchain.Scout.Api.Endpoints;

public static class BlocksEndpoints
{
    public static void MapBlocksEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/v1/blocks")
            .WithTags("Blocks Endpoints");
        
        group.MapPost("/", async (BlockModel block, IBlockRepository repo) =>
        {
            await repo.SaveBlockAsync(block);
            return Results.Created($"/api/blocks/{block.Hash}", block);
        });

        group.MapGet("/{hash}", async (string hash, IBlockRepository repo) =>
        {
            var block = await repo.GetByHashAsync(hash);
            return block is not null ? Results.Ok(block) : Results.NotFound();
        });
        
        group.MapGet("/", async ([AsParameters] BlockQueryParams query, IBlockRepository repo) =>
        {
            var items = await repo.GetPagedAsync(query.Page, query.PageSize);
            var total = await repo.CountAsync();

            var result = new PagedResult<BlockModel>
            {
                Items = items,
                Page = query.Page,
                PageSize = query.PageSize,
                TotalItems = total
            };

            return Results.Ok(result);
        });

    }
}