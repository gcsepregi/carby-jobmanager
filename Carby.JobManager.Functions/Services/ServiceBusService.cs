using System.Runtime.Caching;
using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;

namespace Carby.JobManager.Functions.Services;

public class ServiceBusService : IServiceBusService
{
    private static readonly MemoryCache Cache = MemoryCache.Default;
    
    private readonly ICommonServices _commonServices;

    private static string GetServiceBusConnection(string? jobName)
        => Environment.GetEnvironmentVariable($"{jobName ?? ICommonServices.DefaultJobName}:ServiceBusConnection")!;

    public ServiceBusService(ICommonServices commonServices)
    {
        _commonServices = commonServices;
    }
    
    public async Task<ServiceBusProcessor> CreateProcessorAsync(string? jobName, string queueName, Func<ProcessMessageEventArgs, Task> processMessageCallback, Func<ProcessErrorEventArgs, Task> processErrorCallback)
    {
        await EnsureQueueExistsAsync(jobName, queueName);
        var processor = CreateServiceBusClient(jobName).CreateProcessor(queueName);
        processor.ProcessErrorAsync += processErrorCallback;
        processor.ProcessMessageAsync += processMessageCallback;
        return processor;
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
        var cacheItemKey = $"ServiceBusClient:{jobName}";
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
        var cacheItemKey = $"ServiceBusAdministrationClient:{jobName}";
        if (Cache.Contains(cacheItemKey))
        {
            return (ServiceBusAdministrationClient)Cache.Get(cacheItemKey)!;
        }

        var connection = GetServiceBusConnection(jobName);
        var queueClient = new ServiceBusAdministrationClient(connection);

        return (ServiceBusAdministrationClient)Cache.AddOrGetExisting(cacheItemKey, queueClient, DateTimeOffset.MaxValue) ?? queueClient;
    }
    
}