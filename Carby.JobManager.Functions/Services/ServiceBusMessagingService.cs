using System.Runtime.Caching;
using System.Text.Json;
using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;

namespace Carby.JobManager.Functions.Services;

internal sealed class ServiceBusMessagingService : IMessagingService
{
    private readonly INamedJobCollection _namedJobCollection;
    private static readonly MemoryCache Cache = MemoryCache.Default;
    
    private static string GetServiceBusConnection(string? jobName)
        => Environment.GetEnvironmentVariable($"{jobName ?? ICommonServices.DefaultJobName}:ServiceBusConnection")!;

    public ServiceBusMessagingService(INamedJobCollection namedJobCollection)
    {
        _namedJobCollection = namedJobCollection;
    }
    
    public async Task<IMessageProcessor> CreateProcessorAsync(string? jobName, string queueName, Func<ProcessMessageEventArgs, Task> processMessageCallback, Func<ProcessErrorEventArgs, Task> processErrorCallback)
    {
        await EnsureQueueExistsAsync(jobName, queueName);
        var processor = CreateServiceBusClient(jobName).CreateProcessor(queueName);
        processor.ProcessErrorAsync += processErrorCallback;
        processor.ProcessMessageAsync += processMessageCallback;
        return new ServiceBusMessageProcessorWrapper(processor);
    }

    public async Task TriggerJobAsync(string? jobName)
    {
        var jobDescriptor = _namedJobCollection[jobName ?? ICommonServices.DefaultJobName];
        var messageSender = CreateServiceBusClient(jobName).CreateSender(jobDescriptor.StartTask);
        await messageSender.SendMessageAsync(new ServiceBusMessage(JsonSerializer.Serialize(new TaskRequest())));
    }

    private async Task EnsureQueueExistsAsync(string? jobName, string queueName)
    {
        var administrationClient = CreateServiceBusAdministrationClient(jobName);
        if (!await administrationClient.QueueExistsAsync(queueName))
        {
            await administrationClient.CreateQueueAsync(queueName);
        }
    }

    private ServiceBusClient CreateServiceBusClient(string? jobName)
    {
        var cacheItemKey = $"ServiceBusClient:{jobName ?? ICommonServices.DefaultJobName}";
        if (Cache.Contains(cacheItemKey))
        {
            return (ServiceBusClient)Cache.Get(cacheItemKey)!;
        }
        
        var options = new ServiceBusClientOptions
        {
            TransportType = ServiceBusTransportType.AmqpWebSockets
        };
        var connection = GetServiceBusConnection(jobName);
        var queueClient = new ServiceBusClient(connection, options);

        return (ServiceBusClient)Cache.AddOrGetExisting(cacheItemKey, queueClient, DateTimeOffset.MaxValue) ?? queueClient;
    }
    
    private ServiceBusAdministrationClient CreateServiceBusAdministrationClient(string? jobName)
    {
        var cacheItemKey = $"ServiceBusAdministrationClient:{jobName ?? ICommonServices.DefaultJobName}";
        if (Cache.Contains(cacheItemKey))
        {
            return (ServiceBusAdministrationClient)Cache.Get(cacheItemKey)!;
        }

        var connection = GetServiceBusConnection(jobName);
        var queueClient = new ServiceBusAdministrationClient(connection);

        return (ServiceBusAdministrationClient)Cache.AddOrGetExisting(cacheItemKey, queueClient, DateTimeOffset.MaxValue) ?? queueClient;
    }
    
}