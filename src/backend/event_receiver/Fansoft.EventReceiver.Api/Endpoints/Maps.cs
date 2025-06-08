namespace Fansoft.EventReceiver.Api.Endpoints;

public static class Maps
{
    public static void MapEndpoints(this WebApplication app)
    {
        app.MapEventEndpoints();
        app.MapNftsEndpoints();
        app.MapAppProviderEndpoints();
    }
}