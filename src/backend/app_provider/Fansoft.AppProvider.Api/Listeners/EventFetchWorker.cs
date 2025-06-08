using Fansoft.AppProvider.Api.Data.Entities;
using Fansoft.AppProvider.Api.Data.Repositories;
using Fansoft.AppProvider.Api.Models;
using Microsoft.Extensions.Options;

namespace Fansoft.AppProvider.Api.Listeners;

public class EventFetchWorker(
    IServiceProvider provider,
    ILogger<EventFetchWorker> logger,
    IOptions<RemoteServicesOption> options)
    : BackgroundService
{
    
    private readonly string _eventUrl = options.Value.EventUrl;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = provider.CreateScope();
            var evRepo = scope.ServiceProvider.GetRequiredService<IEventProcessRepository>();
            var http = scope.ServiceProvider.GetRequiredService<IHttpClientFactory>().CreateClient();

            var pendings = await evRepo.GetAllEventPendingsProcesses();

            foreach (var ev in pendings)
            {
                try
                {
                    var response = await http.GetAsync($"{_eventUrl}/{ev.EventHash}", stoppingToken);
                    response.EnsureSuccessStatusCode();

                    var json = await response.Content.ReadAsStringAsync(stoppingToken);
                    ev.OriginalPayload = json;
                    ev.Status = StatusProcess.Completed;
                    ev.ProcessedAt = DateTime.UtcNow;
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "Erro ao buscar evento {Hash}", ev.EventHash);
                    ev.Status = StatusProcess.Failed;
                    ev.FailureReason = ex.Message;
                }
                await evRepo.SaveAsync(stoppingToken);
            }
            await Task.Delay(TimeSpan.FromSeconds(60), stoppingToken);
        }
    }
}