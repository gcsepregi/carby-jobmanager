using Azure.Messaging.ServiceBus;

namespace Carby.JobManager.Functions.Services;

public interface IServiceBusService
{
    public Task<ServiceBusProcessor> CreateProcessorAsync(
        string? jobName,
        string queueName,
        Func<ProcessMessageEventArgs, Task> processMessageCallback,
        Func<ProcessErrorEventArgs, Task> processErrorCallback
        );
}