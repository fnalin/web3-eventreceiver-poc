using Fansoft.AppProvider.Api.Models;

namespace Fansoft.AppProvider.Api.Endpoints;

public static class WebHooksEndpoints
{
    public static void MapWebHooksEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/v1/webhooks")
            .WithTags("Event Webhooks");

        group.MapPost("/nfts", (EventNotificationInput input, CancellationToken cancellationToken) =>
        {
            if (string.IsNullOrWhiteSpace(input.EventHash))
                return Results.BadRequest("EventHash is required.");

            // Processamento aqui

            return Results.Ok(new { message = "Notificação recebida com sucesso." });
            
        });
        
    }
}