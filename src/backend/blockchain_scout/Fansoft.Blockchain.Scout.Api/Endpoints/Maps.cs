namespace Fansoft.Blockchain.Scout.Api.Endpoints;

public static class Maps
{
    public static void MapEndpoints(this WebApplication app)
    {
        app.MapBlocksEndpoints();
    }
}