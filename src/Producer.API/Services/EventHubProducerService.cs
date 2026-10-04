using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using System.Text;
using System.Text.Json;
using Producer.API.Models;

namespace Producer.API.Services;

public class EventHubProducerService : IAsyncDisposable
{
    private readonly EventHubProducerClient _client;
    private readonly ILogger<EventHubProducerService> _logger;

    public EventHubProducerService(IConfiguration config, ILogger<EventHubProducerService> logger)
    {
        var connectionString = config["EventHub:ConnectionString"];
        var eventHubName = config["EventHub:Name"] ?? "order-events";
        _client = new EventHubProducerClient(connectionString, eventHubName);
        _logger = logger;
    }

    public async Task SendEventAsync(OrderEvent orderEvent)
    {
        var eventData = new EventData(
            Encoding.UTF8.GetBytes(JsonSerializer.Serialize(orderEvent)));

        using var batch = await _client.CreateBatchAsync();
        batch.TryAdd(eventData);
        await _client.SendAsync(batch);

        _logger.LogInformation("Event sent to EventHub: {EventId}", orderEvent.EventId);
    }

    public async ValueTask DisposeAsync() => await _client.DisposeAsync();
}