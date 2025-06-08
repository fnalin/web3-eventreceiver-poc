using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Fansoft.DigitalTwin.Api.Exceptions;

namespace Fansoft.DigitalTwin.Api.Services;

public class EventApiService(HttpClient httpClient, IConfiguration config) : IEventApiService
{
    public async Task<string> SendEventAsync(object payload)
    {
        var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
        var uri = "?twinName=dt1";

        try
        {
            var response = await httpClient.PostAsync(uri, content);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var doc = JsonDocument.Parse(json);
            return doc.RootElement
                .GetProperty("dataEvent")
                .GetProperty("transactionHash")
                .GetString()!;
        }
        catch (HttpRequestException ex)
        {
            throw new EventApiUnavailableException("API de destino está indisponível.", ex);
        }
        catch (TaskCanceledException ex)
        {
            throw new EventApiUnavailableException("A requisição para a API de destino expirou.", ex);
        }
    }
}

public interface IEventApiService
{
    Task<string> SendEventAsync(object payload);
}