using System.Diagnostics;
using Azure;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using Carby.JobManager.Functions.Tracing;

namespace Carby.JobManager.Functions.Services;

internal sealed class StorageQueueMessageProcessor : IMessageProcessor
{
    private readonly QueueClient _queueClient;
    private readonly QueueClient _poisonQueueClient;
    private readonly TimeSpan _messageVisibilityTimeout;
    private readonly int _parallelMessageCount;
    private readonly int _messageRetryCount;
    private readonly StorageQueueMessagingService _storageQueueMessagingService;
    private bool _stopRequested;
    private Task? _receiverTask;

    public event Func<TaskRequest, CancellationToken, Task<MessageProcessorResult>>? OnMessage;
    public event Func<Exception, Task<bool>>? OnError;
    
    public StorageQueueMessageProcessor(QueueClient queueClient,
        QueueClient poisonQueueClient,
        TimeSpan messageVisibilityTimeout,
        int parallelMessageCount,
        int messageRetryCount, 
        StorageQueueMessagingService storageQueueMessagingService)
    {
        _queueClient = queueClient;
        _poisonQueueClient = poisonQueueClient;
        _messageVisibilityTimeout = messageVisibilityTimeout;
        _parallelMessageCount = parallelMessageCount;
        _messageRetryCount = messageRetryCount;
        _storageQueueMessagingService = storageQueueMessagingService;
    }

    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }

    public Task StartProcessingAsync(CancellationToken cancellationToken)
    {
        if (OnMessage == null)
        {
            throw new InvalidOperationException("Cannot start processing as no message handler had been provided");
        }
        _receiverTask = StartReceivingAsync(cancellationToken);
        return Task.CompletedTask;
    }

    private Task StartReceivingAsync(CancellationToken cancellationToken)
    {
        return Task.Run(async () =>
        {
            var inFlight = new HashSet<Task>();
            while (!cancellationToken.IsCancellationRequested && !_stopRequested)
            {
                if (inFlight.Count >= _parallelMessageCount)
                {
                    var finishedItem = await Task.WhenAny(inFlight);
                    inFlight.Remove(finishedItem);
                }

                var message = await GetNextMessage(cancellationToken);
                
                if (message != null)
                { 
                    inFlight.Add(TryProcessMessageAsync(message, cancellationToken));
                }
            }
        }, cancellationToken);
    }

    private async Task<QueueMessage?> GetNextMessage(CancellationToken cancellationToken)
    {
        Response<QueueMessage>? queueMessage = null;
        try
        {
            queueMessage = await _queueClient.ReceiveMessageAsync(_messageVisibilityTimeout, cancellationToken: cancellationToken);

            if (queueMessage?.Value == null)
            {
                //TODO: implement configurable delay
                await Task.Delay(100, cancellationToken);
                return null;
            }

            return queueMessage.Value;
        }
        catch (Exception e)
        {
            await ProcessExceptionAsync(e, queueMessage?.Value, cancellationToken);
            
            if (!cancellationToken.IsCancellationRequested)
            {
                //TODO: implement configurable delay
                await Task.Delay(100, cancellationToken);
            }

            return null;
        }
    }

    private async Task TryProcessMessageAsync(QueueMessage message, CancellationToken cancellationToken)
    {
        Activity? activity = null;
        try
        {
            var envelope = message.Body.ToObjectFromJson<TaskRequestEnvelope>();
            activity = _queueClient.Name.CreateActivity(ActivityKind.Client);
            activity.FillActivityFromDistributedContext(envelope.Headers);
            activity.AddTag(ICommonServices.TaskInstanceId, envelope.Headers[ICommonServices.TaskInstanceId]);
            activity.AddTag(ICommonServices.CurrentTaskName, envelope.Headers[ICommonServices.CurrentTaskName]);
            activity.OnActivityImport();
            activity.StartActivity();

            var result = await OnMessage!(new TaskRequest(), cancellationToken);

            var jobName = Activity.Current?.GetBaggageItem(ICommonServices.CurrentJobName) ?? throw new InvalidOperationException("Activity must already be started and baggage and tags filled");
            var taskName = (string?)Activity.Current.GetTagItem(ICommonServices.CurrentTaskName) ?? throw new InvalidOperationException("Activity must already be started and baggage and tags filled");
        
            if (result.Succeeded)
            {
                await _storageQueueMessagingService.TriggerNextTaskAsync(envelope, jobName, taskName);
                await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, cancellationToken);
            }
            else
            {
                await _storageQueueMessagingService.TriggerFailureHandlerTaskAsync(jobName, taskName);
                await ProcessExceptionAsync(result.Exception!, message, cancellationToken);
            }
        }
        catch (Exception e)
        {
            await ProcessExceptionAsync(e, message, cancellationToken);
        }
        finally
        {
            activity?.StopActivity();
        }
    }

    private async Task ProcessExceptionAsync(Exception exception, QueueMessage? message, CancellationToken cancellationToken)
    {
        if (message == null)
        {
            throw new ArgumentNullException(nameof(message));
        }
        
        var isFatalError = OnError != null && await OnError(exception);
        
        if (isFatalError || message.DequeueCount >= _messageRetryCount)
        {
            await _poisonQueueClient.SendMessageAsync(message.Body, cancellationToken: cancellationToken);
            await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, cancellationToken);
        }
    }

    public Task StopProcessingAsync(CancellationToken cancellationToken = default)
    {
        _stopRequested = true;
        return _receiverTask ?? Task.CompletedTask;
    }
}

internal class MessageProcessorResult
{
    public bool Succeeded { get; set; }
    public Exception? Exception { get; set; }
}