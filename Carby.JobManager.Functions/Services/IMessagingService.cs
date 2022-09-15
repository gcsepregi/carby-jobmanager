using Azure.Messaging.ServiceBus;

namespace Carby.JobManager.Functions.Services;

internal interface IMessagingService
{
    public Task<IMessageProcessor> CreateProcessorAsync(
        string? jobName,
        string queueName,
        Func<TaskRequest, CancellationToken, Task<MessageProcessorResult>> processMessageCallback,
        Func<Exception, Task<bool>> processErrorCallback
        );

    Task TriggerJobAsync(string? jobName);
}