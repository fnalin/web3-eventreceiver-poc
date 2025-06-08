using Fansoft.AppProvider.Api.Data.Entities;
using Fansoft.AppProvider.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Fansoft.AppProvider.Api.Data.Repositories;

public interface IEventProcessRepository
{
    Task<EventProcess?> GetEventProcessById(int id);
    Task<EventResponsePaginated> GettAllEventProcessesPaginated(int page, int pageSize, CancellationToken cancellationToken);
    Task<IEnumerable<EventProcess>> GetAllEventProcesses();
    Task<IEnumerable<EventProcess>> GetAllEventPendingsProcesses();
    
    Task AddEventProcess(EventProcess eventProcess);
    Task SaveAsync(CancellationToken cancellationToken);
}

public class EventProcessRepository (EventProcessDataContext ctx) : IEventProcessRepository
{
    public async Task<EventProcess?> GetEventProcessById(int id) => 
        await ctx.EventProcesses.FirstOrDefaultAsync(x => x.Id == id);

    public async Task<EventResponsePaginated> GettAllEventProcessesPaginated(int page, int pageSize, CancellationToken cancellationToken)
    {
        var totalCount = await ctx.EventProcesses.CountAsync(cancellationToken);
        var items = await ctx.EventProcesses
            .OrderByDescending(x => x.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new EventResponsePaginated
        {
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize,
            Items = items
        };
    }

    public async Task<IEnumerable<EventProcess>> GetAllEventProcesses() =>
        await ctx.EventProcesses.ToListAsync();
    
    public async Task<IEnumerable<EventProcess>> GetAllEventPendingsProcesses() =>
        await ctx.EventProcesses.Where(x=>x.Status == StatusProcess.Pending)
            .Take(10).ToListAsync();
    
    public async Task AddEventProcess(EventProcess eventProcess)
    {
        await ctx.EventProcesses.AddAsync(eventProcess);
        await ctx.SaveChangesAsync();
    }

    public async Task SaveAsync(CancellationToken cancellationToken) =>
        await ctx.SaveChangesAsync(cancellationToken);
}

