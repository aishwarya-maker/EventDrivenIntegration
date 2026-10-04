using Azure.Messaging.EventHubs;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;
using Processor.Function.Models;

namespace Processor.Function.Functions;

public class EventHubTriggerFunction
{
    private readonly ServiceBusClient _serviceBusClient;
    private readonly ILogger<EventHubTriggerFunction> _logger;

    public EventHubTriggerFunction(ServiceBusClient serviceBusClient,
        ILogger<EventHubTriggerFunction> logger)
    {
        _serviceBusClient = serviceBusClient;
        _logger = logger;
    }

    [Function("ProcessOrderEvent")]
    public async Task Run(
        [EventHubTrigger("order-events", Connection = "EventHub__ConnectionString")] 
        EventData[] events)
    {
        var sender = _serviceBusClient.CreateSender("processed-orders");

        foreach (var eventData in events)
        {
            var body = Encoding.UTF8.GetString(eventData.Body.ToArray());
            _logger.LogInformation("Processing event: {Body}", body);

            // Simulate processing
            var processed = new ProcessedEvent
            {
                OriginalPayload = body,
                ProcessedAt = DateTime.UtcNow,
                Status = "Processed"
            };

            var message = new ServiceBusMessage(
                JsonSerializer.Serialize(processed));

            await sender.SendMessageAsync(message);
            _logger.LogInformation("Sent to Service Bus queue: processed-orders");
        }
    }
}