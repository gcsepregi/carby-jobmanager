using System.Text.Json;
using Azure.Storage.Queues;

namespace Carby.JobManager.Functions.Services;

internal sealed class StorageQueueMessagingService : StorageManagerServiceBase, IMessagingService
{
    private readonly INamedJobCollection _namedJobCollection;

    public StorageQueueMessagingService(INamedJobCollection namedJobCollection)
    {
        _namedJobCollection = namedJobCollection;
    }
    
    public async Task<IMessageProcessor> CreateProcessorAsync(string? jobName, 
        string queueName, 
        Func<TaskRequest, CancellationToken, Task<MessageProcessorResult>> processMessageCallback, 
        Func<Exception, Task<bool>> processErrorCallback
        )
    {
        var queueClient = new QueueClient(GetStorageConnection(), queueName.ToLowerInvariant());
        var poisonQueueClient = new QueueClient(GetStorageConnection(), $"{queueName.ToLowerInvariant()}poisoned");
        await queueClient.CreateIfNotExistsAsync();
        await poisonQueueClient.CreateIfNotExistsAsync();
        return new StorageQueueMessageProcessor(queueClient, 
            poisonQueueClient, 
            processMessageCallback, 
            processErrorCallback,
            GetMessageVisibilityTimeout(queueName),
            GetParallelMessageCount(queueName),
            GetMessageRetryCount(queueName)
            );
    }

    public async Task TriggerJobAsync(string? jobName)
    {
        var jobDescriptor = _namedJobCollection[jobName ?? ICommonServices.DefaultJobName];
        var queueClient = new QueueClient(GetStorageConnection(), jobDescriptor.StartTask?.ToLower());
        await queueClient.SendMessageAsync(new BinaryData(JsonSerializer.Serialize(new TaskRequest())));
    }
}