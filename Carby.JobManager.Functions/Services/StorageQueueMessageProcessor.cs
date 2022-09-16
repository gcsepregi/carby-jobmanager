using Azure;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;

namespace Carby.JobManager.Functions.Services;

internal sealed class StorageQueueMessageProcessor : IMessageProcessor
{
    private readonly QueueClient _queueClient;
    private readonly QueueClient _poisonQueueClient;
    private readonly TimeSpan _messageVisibilityTimeout;
    private readonly int _parallelMessageCount;
    private readonly int _messageRetryCount;
    private bool _stopRequested;
    private Task? _receiverTask;

    public event Func<TaskRequest, CancellationToken, Task<MessageProcessorResult>>? OnMessage;
    public event Func<Exception, Task<bool>>? OnError;
    
    public StorageQueueMessageProcessor(QueueClient queueClient,
        QueueClient poisonQueueClient,
        TimeSpan messageVisibilityTimeout,
        int parallelMessageCount,
        int messageRetryCount)
    {
        _queueClient = queueClient;
        _poisonQueueClient = poisonQueueClient;
        _messageVisibilityTimeout = messageVisibilityTimeout;
        _parallelMessageCount = parallelMessageCount;
        _messageRetryCount = messageRetryCount;
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
                await Task.Delay(1000, cancellationToken);
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
                await Task.Delay(1000, cancellationToken);
            }

            return null;
        }
    }

    private async Task TryProcessMessageAsync(QueueMessage message, CancellationToken cancellationToken)
    {
        try
        {
            var result = await OnMessage!(new TaskRequest
            {
                RequestId = message.MessageId,
                Message = message.Body,
                DequeueCount = message.DequeueCount,
                InsertedOn = message.InsertedOn,
                ExpiresOn = message.ExpiresOn,
            }, cancellationToken);
            
            if (result.Succeeded)
            {
                await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, cancellationToken);
            }
            else
            {
                await ProcessExceptionAsync(result.Exception!, message, cancellationToken);
            }
        }
        catch (Exception e)
        {
            await ProcessExceptionAsync(e, message, cancellationToken);
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