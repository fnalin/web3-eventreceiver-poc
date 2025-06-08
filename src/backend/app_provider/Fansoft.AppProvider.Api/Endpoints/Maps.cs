namespace Fansoft.AppProvider.Api.Endpoints;

public static class Maps
{
    public static void MapEndpoints(this WebApplication app)
    {
        app.MapEventEndpoints();
        app.MapWebHooksEndpoints();
    }
}