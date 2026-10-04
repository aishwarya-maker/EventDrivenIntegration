using Azure.Messaging.ServiceBus;
using Consumer.API.Data;
using Consumer.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Consumer.API.Services;

public class ServiceBusConsumerService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ServiceBusClient _client;
    private readonly ILogger<ServiceBusConsumerService> _logger;

    public ServiceBusConsumerService(ServiceBusClient client,
        IServiceScopeFactory scopeFactory,
        ILogger<ServiceBusConsumerService> logger)
    {
        _client = client;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var processor = _client.CreateProcessor("processed-orders");

        processor.ProcessMessageAsync += async args =>
        {
            var body = args.Message.Body.ToString();
            _logger.LogInformation("Received from Service Bus: {Body}", body);

            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            db.EventResults.Add(new EventResult
            {
                Payload = body,
                Status = "Consumed"
            });
            await db.SaveChangesAsync(stoppingToken);
            await args.CompleteMessageAsync(args.Message, stoppingToken);
        };

        processor.ProcessErrorAsync += args =>
        {
            _logger.LogError(args.Exception, "Service Bus error");
            return Task.CompletedTask;
        };

        await processor.StartProcessingAsync(stoppingToken);
        await Task.Delay(Timeout.Infinite, stoppingToken);
    }
}