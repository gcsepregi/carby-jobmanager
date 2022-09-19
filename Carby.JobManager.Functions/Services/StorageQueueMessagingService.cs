using System.Diagnostics;
using System.Text.Json;
using Azure.Storage.Queues;

namespace Carby.JobManager.Functions.Services;

internal sealed class StorageQueueMessagingService : StorageManagerServiceBase, IMessagingService
{
    private readonly INamedJobCollection _namedJobCollection;
    private readonly IJobContextManagerService _jobContextManagerService;

    public StorageQueueMessagingService(INamedJobCollection namedJobCollection, IJobContextManagerService jobContextManagerService)
    {
        _namedJobCollection = namedJobCollection;
        _jobContextManagerService = jobContextManagerService;
    }
    
    public async Task<IMessageProcessor> CreateProcessorAsync(string queueName)
    {
        var queueClient = new QueueClient(GetStorageConnection(), queueName.ToLowerInvariant());
        var poisonQueueClient = new QueueClient(GetStorageConnection(), $"{queueName.ToLowerInvariant()}poisoned");
        await queueClient.CreateIfNotExistsAsync();
        await poisonQueueClient.CreateIfNotExistsAsync();
        return new StorageQueueMessageProcessor(queueClient, 
            poisonQueueClient, 
            GetMessageVisibilityTimeout(queueName),
            GetParallelMessageCount(queueName),
            GetMessageRetryCount(queueName)
            );
    }

    public async Task TriggerJobAsync(string jobName)
    {
        var jobDescriptor = _namedJobCollection[jobName ?? ICommonServices.DefaultJobName];
        await TriggerTaskAsync(jobDescriptor.StartTask!);
    }

    public async Task TriggerNextTaskAsync(string jobName, string currentTask)
    {
        var nextTask = await FindNextTaskAsync(jobName, currentTask);
        if (nextTask != null)
        {
            await TriggerTaskAsync(nextTask);
        }
    }

    public async Task TriggerFailureHandlerTaskAsync(string jobName, string currentTask)
    {
        var jobDescriptor = _namedJobCollection[jobName ?? ICommonServices.DefaultJobName];

    }
    
    public async Task TriggerTaskAsync(string taskName)
    {
        var queueClient = new QueueClient(GetStorageConnection(), taskName.ToLower());
        var taskRequest = new TaskRequestEnvelope
        {
            Headers =
            {
                [ICommonServices.TaskInstanceIdKey] = ActivityTraceId.CreateRandom().ToHexString(),
                [ICommonServices.CurrentTaskNameKey] = taskName
            }
        };

        DistributedContextPropagator.Current.Inject(Activity.Current, taskRequest, (carrier, name, value) =>
        {
            var request = (TaskRequestEnvelope)carrier!;
            request.Headers[name] = value;
        });
        
        await queueClient.SendMessageAsync(new BinaryData(JsonSerializer.Serialize(taskRequest)));
    }
    
    private async Task<string?> FindNextTaskAsync(string jobName, string currentTask)
    {
        var jobDescriptor = _namedJobCollection[jobName];

        if (jobDescriptor.CleanUpTask == currentTask)
        {
            return null;
        }
        
        if (jobDescriptor.EndTask == currentTask || jobDescriptor.HandleFailureTask == currentTask)
        {
            return jobDescriptor.CleanUpTask;
        }

        string? defaultTask = null;
        string? guardedTask = null;

        var possibleTransitions = jobDescriptor.Transitions[currentTask];
        var jobContext = await _jobContextManagerService.ReadJobContextAsync();
        
        foreach (var transition in possibleTransitions)
        {
            if (transition.When == null)
            {
                defaultTask ??= transition.ToTask;
            } 
            else if (transition.When(jobContext))
            {
                guardedTask = transition.ToTask;
                break;
            }
        }

        return guardedTask ?? defaultTask;
    }
    
}