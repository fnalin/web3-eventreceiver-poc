using System.Text.Json;
using Fansoft.DigitalTwin.Api.Data;
using Fansoft.DigitalTwin.Api.Data.Entities;
using Fansoft.DigitalTwin.Api.Exceptions;
using Fansoft.DigitalTwin.Api.Models;
using Fansoft.DigitalTwin.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Fansoft.DigitalTwin.Api.Extensions;

public static class EndpointExtension
{
    public static void MapEventEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/v1/events")
            .WithTags("Events");

        group.MapPost("/", async (
            [FromBody] JsonElement payload,
            AppDbContext db,
            EventApiService externalApi,
            ILogger<Program> logger) =>
        {
            try
            {
                var externalId = await externalApi.SendEventAsync(payload);

                var stored = new StoredEvent
                {
                    ExternalId = externalId,
                    OriginalPayload = payload.GetRawText()
                };

                db.StoredEvents.Add(stored);
                await db.SaveChangesAsync();

                return Results.Ok(new { internalId = stored.Id, stored.ExternalId });
            }
            catch (EventApiUnavailableException ex)
            {
                logger.LogWarning(ex, "API externa indisponível ao tentar enviar evento. Payload: {Payload}", payload.GetRawText());
                return Results.StatusCode(StatusCodes.Status503ServiceUnavailable);
            }
        });

        group.MapGet("/{id}", async (
            string id,
            AppDbContext db) =>
        {
            var ev = await db.StoredEvents.FirstOrDefaultAsync(x=>x.ExternalId == id);
            return ev is null
                ? Results.NotFound()
                : Results.Ok(JsonDocument.Parse(ev.OriginalPayload).RootElement);
        });
        
        group.MapGet("/", async (
            [FromQuery] int page,
            [FromQuery] int pageSize,
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate,
            AppDbContext db) =>
        {
            if (page <= 0 || pageSize <= 0)
                return Results.BadRequest("Parâmetros de paginação inválidos.");

            var query = db.StoredEvents.AsQueryable();

            if (startDate.HasValue)
                query = query.Where(e => e.CreatedAt >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(e => e.CreatedAt <= endDate.Value);

            var total = await query.CountAsync();

            var rawEvents = await query
                .OrderByDescending(e => e.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            
            var items = rawEvents.Select(e => new EventListItemDto(
                e.Id,
                e.ExternalId,
                e.CreatedAt,
                JsonDocument.Parse(e.OriginalPayload).RootElement
            )).ToList();

            return Results.Ok(new PagedResult<EventListItemDto>(total, page, pageSize, items));
        });
    }
}