using Fansoft.EventReceiver.Api.Data.Repositories;

namespace Fansoft.EventReceiver.Api.Endpoints;

public static class AppProviderEndpoints
{
    public static void MapAppProviderEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/v1/app-providers")
            .WithTags("App-Providers Endpoints");

        group.MapGet("/{eventHash}", async (
            string eventHash,
            IEventRepository eventRepository,
            INftPurchaseRepository nftPurchaseRepository,
            IHttpClientFactory httpClientFactory) =>
        {
            var eventDoc = await eventRepository.GetByEventHashAsync(eventHash);

            if (eventDoc is null)
                return Results.NotFound("Event not found");

            var purchaseData = await nftPurchaseRepository.GetByTokenIdAsync(eventDoc.TokenId);
            var appProvierWallet = "0x90F79bf6EB2c4f870365E785982E1f101E93b906".ToLowerInvariant();
            if (purchaseData is null || purchaseData.Wallets.All(w => !w.Equals(appProvierWallet, StringComparison.InvariantCultureIgnoreCase)))
                return Results.NotFound("Purchase data not found for the given token ID");

            //Obter o evento via Get do DigitalTwin
            var url = $"{eventDoc.DigitalTwin.CallBackUrl}/{eventHash}";

            try
            {
                var client = httpClientFactory.CreateClient();
                var response = await client.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                    return Results.StatusCode((int)response.StatusCode);

                var content = await response.Content.ReadAsStringAsync();

                return Results.Content(content, "application/json");
            }
            catch (Exception ex)
            {
                return Results.Problem($"Erro ao acessar o Digital Twin: {ex.Message}");
            }
        });
    }
}