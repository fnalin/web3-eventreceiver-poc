using System.Text.Json;

namespace Fansoft.DigitalTwin.Api.Models;

public record EventListItemDto(
    int Id,
    string ExternalId,
    DateTime CreatedAt,
    JsonElement OriginalPayload
);

public record PagedResult<T>(
    int TotalCount,
    int Page,
    int PageSize,
    IEnumerable<T> Items
);