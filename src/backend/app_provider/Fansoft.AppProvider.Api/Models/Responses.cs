using Fansoft.AppProvider.Api.Data.Entities;

namespace Fansoft.AppProvider.Api.Models;

public class EventResponsePaginated
{
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public IEnumerable<EventProcess> Items { get; set; } = [];
}
