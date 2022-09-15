using System.Runtime.Caching;
using System.Text.Json;
using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;

namespace Carby.JobManager.Functions.Services;

internal sealed class ServiceBusMessagingService : IMessagingService
{
    private readonly INamedJobCollection _namedJobCollection;
    private static readonly MemoryCache Cache = MemoryCache.Default;
    
    private static string GetServiceBusConnection()
        => Environment.GetEnvironmentVariable($"CarbyJobManager:ServiceBusConnection")!;

    public ServiceBusMessagingService(INamedJobCollection namedJobCollection)
    {
        _namedJobCollection = namedJobCollection;
    }
    
    public async Task<IMessageProcessor> CreateProcessorAsync(string? jobName, string queueName, Func<TaskRequest, CancellationToken, Task<MessageProcessorResult>> processMessageCallback, Func<Exception, Task> processErrorCallback)
    {
        await EnsureQueueExistsAsync(queueName);
        var processor = CreateServiceBusClient().CreateProcessor(queueName);
        processor.ProcessErrorAsync += (message) => processErrorCallback(message.Exception);
        processor.ProcessMessageAsync += (message) => processMessageCallback(new TaskRequest(), message.CancellationToken);
        return new ServiceBusMessageProcessorWrapper(processor);
    }

    public async Task TriggerJobAsync(string? jobName)
    {
        var jobDescriptor = _namedJobCollection[jobName ?? ICommonServices.DefaultJobName];
        var messageSender = CreateServiceBusClient().CreateSender(jobDescriptor.StartTask);
        await messageSender.SendMessageAsync(new ServiceBusMessage(JsonSerializer.Serialize(new TaskRequest())));
    }

    private async Task EnsureQueueExistsAsync(string queueName)
    {
        var administrationClient = CreateServiceBusAdministrationClient();
        if (!await administrationClient.QueueExistsAsync(queueName))
        {
            await administrationClient.CreateQueueAsync(queueName);
        }
    }

    private ServiceBusClient CreateServiceBusClient()
    {
        var cacheItemKey = $"ServiceBusClient:{ICommonServices.DefaultJobName}";
        if (Cache.Contains(cacheItemKey))
        {
            return (ServiceBusClient)Cache.Get(cacheItemKey)!;
        }
        
        var options = new ServiceBusClientOptions
        {
            TransportType = ServiceBusTransportType.AmqpWebSockets
        };
        var connection = GetServiceBusConnection();
        var queueClient = new ServiceBusClient(connection, options);

        return (ServiceBusClient)Cache.AddOrGetExisting(cacheItemKey, queueClient, DateTimeOffset.MaxValue) ?? queueClient;
    }
    
    private ServiceBusAdministrationClient CreateServiceBusAdministrationClient()
    {
        var cacheItemKey = $"ServiceBusAdministrationClient:{ICommonServices.DefaultJobName}";
        if (Cache.Contains(cacheItemKey))
        {
            return (ServiceBusAdministrationClient)Cache.Get(cacheItemKey)!;
        }

        var connection = GetServiceBusConnection();
        var queueClient = new ServiceBusAdministrationClient(connection);

        return (ServiceBusAdministrationClient)Cache.AddOrGetExisting(cacheItemKey, queueClient, DateTimeOffset.MaxValue) ?? queueClient;
    }
    
}