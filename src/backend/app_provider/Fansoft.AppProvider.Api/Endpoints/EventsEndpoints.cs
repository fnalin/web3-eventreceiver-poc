using Fansoft.AppProvider.Api.Data.Repositories;

namespace Fansoft.AppProvider.Api.Endpoints;

public static class EventsEndpoints
{
    public static void MapEventEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/v1/events")
            .WithTags("Events");
        
        group.MapGet("/",  async (int page, int pageSize
            ,IEventProcessRepository eventProcessRepository,CancellationToken cancellation) =>
        {
            var eventProcesses = await eventProcessRepository.GettAllEventProcessesPaginated(page, pageSize, cancellation);
            return Results.Ok(eventProcesses);
        });
        
        group.MapGet("/{id:int}",  async (int id, IEventProcessRepository eventProcessRepository) =>
        {
            var eventProcesses = await eventProcessRepository.GetEventProcessById(id);
            return eventProcesses is null ? Results.NotFound() : Results.Ok(eventProcesses);
        });
    }
}