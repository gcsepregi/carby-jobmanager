using Azure.Messaging.ServiceBus;

namespace Carby.JobManager.Functions.Services;

public interface IMessagingService
{
    public Task<IMessageProcessor> CreateProcessorAsync(
        string? jobName,
        string queueName,
        Func<ProcessMessageEventArgs, Task> processMessageCallback,
        Func<ProcessErrorEventArgs, Task> processErrorCallback
        );
}