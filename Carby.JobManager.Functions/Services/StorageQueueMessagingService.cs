using System.Diagnostics;
using System.Text.Json;
using Azure.Storage.Queues;
using Carby.JobManager.Functions.JobModel;

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
            GetMessageRetryCount(queueName),
            this
            );
    }

    public async Task TriggerJobAsync(string jobName)
    {
        var jobDescriptor = _namedJobCollection[jobName];
        await TriggerTaskAsync(jobDescriptor.StartTask);
    }

    public async Task TriggerNextTaskAsync(TaskRequestEnvelope taskRequest, string jobName, string currentTask)
    {
        var jobContext = await _jobContextManagerService.ReadJobContextAsync();
        var nextTask = FindNextTaskAsync(jobName, currentTask, jobContext);
        if (nextTask != null)
        {
            if (nextTask.FanOut != null)
            {
                await TriggerTaskAsync(nextTask.ToTask, taskRequest, nextTask.FanOut(jobContext));
            }
            else
            {
                await TriggerTaskAsync(nextTask.ToTask, taskRequest);
            }
        }
    }

    public async Task TriggerFailureHandlerTaskAsync(string jobName, string currentTask)
    {
        var jobDescriptor = _namedJobCollection[jobName];

    }

    private static async Task TriggerTaskAsync(string taskName, TaskRequestEnvelope? originalRequest = null, int triggerCount = 1)
    {
        var queueClient = new QueueClient(GetStorageConnection(), taskName.ToLower());
        var fanGroupId = ActivityTraceId.CreateRandom().ToHexString();
        for (var i = 0; i < triggerCount; i++) {
            var taskRequest = new TaskRequestEnvelope
            {
                Headers =
                {
                    [ICommonServices.TaskInstanceId] = ActivityTraceId.CreateRandom().ToHexString(),
                    [ICommonServices.CurrentTaskName] = taskName
                }
            };

            if (triggerCount > 1)
            {
                taskRequest.Headers[ICommonServices.FanId] = $"{i}";
                taskRequest.Headers[ICommonServices.FanGroupId] = fanGroupId;
                taskRequest.Headers[ICommonServices.FanGroupSize] = $"{triggerCount}";
            }

            DistributedContextPropagator.Current.Inject(Activity.Current, taskRequest, (carrier, name, value) =>
            {
                var request = (TaskRequestEnvelope)carrier!;
                request.Headers[name] = value;
            });

            await queueClient.SendMessageAsync(new BinaryData(JsonSerializer.Serialize(taskRequest)));
        }
    }
    
    private TransitionDescriptor? FindNextTaskAsync(string jobName, string currentTask,
        IJobContext jobContext)
    {
        var jobDescriptor = _namedJobCollection[jobName];

        if (jobDescriptor.CleanUpTask == currentTask)
        {
            return null;
        }
        
        if (jobDescriptor.EndTask == currentTask || jobDescriptor.HandleFailureTask == currentTask)
        {
            return new TransitionDescriptor
            {
                FromTask = currentTask,
                ToTask = jobDescriptor.CleanUpTask
            };
        }

        TransitionDescriptor? defaultTask = null;
        TransitionDescriptor? guardedTask = null;

        var possibleTransitions = jobDescriptor.Transitions[currentTask];
        
        foreach (var transition in possibleTransitions)
        {
            if (transition.When == null)
            {
                defaultTask ??= transition;
            } 
            else if (transition.When(jobContext))
            {
                guardedTask = transition;
                break;
            }
        }

        return guardedTask ?? defaultTask;
    }
    
}