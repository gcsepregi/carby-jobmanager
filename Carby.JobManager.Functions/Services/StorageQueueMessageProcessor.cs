using System.Collections.Concurrent;
using Azure;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;

namespace Carby.JobManager.Functions.Services;

internal sealed class StorageQueueMessageProcessor : IMessageProcessor
{
    private readonly QueueClient _queueClient;
    private readonly QueueClient _poisonQueueClient;
    private readonly BlockingCollection<QueueMessage> _blockingQueue;
    private readonly TimeSpan _messageVisibilityTimeout;
    private readonly int _messageRetryCount;
    private bool _stopRequested;
    private Task? _emitterTask;
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
        _messageRetryCount = messageRetryCount;
        _blockingQueue = new BlockingCollection<QueueMessage>(parallelMessageCount);
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
        _emitterTask = StartEmittingMessages(cancellationToken);
        _receiverTask = StartReceivingAsync(cancellationToken);
        return Task.CompletedTask;
    }

    private Task StartReceivingAsync(CancellationToken cancellationToken)
    {
        var receiverTask = Task.Run(async () =>
        {
            while (!cancellationToken.IsCancellationRequested && !_stopRequested)
            {
                await GetNextMessage(cancellationToken);
            }

            _blockingQueue.CompleteAdding();
        }, cancellationToken);
        return receiverTask;
    }

    private async Task GetNextMessage(CancellationToken cancellationToken)
    {
        Response<QueueMessage>? queueMessage = null;
        try
        {
            queueMessage = await _queueClient.ReceiveMessageAsync(_messageVisibilityTimeout, cancellationToken: cancellationToken);

            if (queueMessage?.Value == null)
            {
                //TODO: implement configurable delay
                await Task.Delay(1000, cancellationToken);
                return;
            }

            _blockingQueue.Add(queueMessage, cancellationToken);
        }
        catch (Exception e)
        {
            await ProcessExceptionAsync(e, queueMessage?.Value, cancellationToken);
            
            if (!cancellationToken.IsCancellationRequested)
            {
                //TODO: implement configurable delay
                await Task.Delay(1000, cancellationToken);
            }
        }
    }

    private Task StartEmittingMessages(CancellationToken cancellationToken)
    {
        var emitterTask = Task.Run(async () =>
        {
            while (!_blockingQueue.IsCompleted)
            {
                QueueMessage? message = null;
                try
                {
                    message = _blockingQueue.Take();
                }
                catch (InvalidOperationException)
                {
                    // Queue is empty and was marked as complete
                    // Nothing to do, while will terminate
                }
                catch (OperationCanceledException)
                {
                    // Queue is empty and was marked as complete
                    // Nothing to do, while will terminate
                }

                if (message != null)
                {
                    await TryProcessMessageAsync(message, cancellationToken);
                }
            }
        }, cancellationToken);

        return emitterTask;
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
        return Task.WhenAll(_receiverTask ?? Task.CompletedTask, _emitterTask ?? Task.CompletedTask);
    }
}

internal class MessageProcessorResult
{
    public bool Succeeded { get; set; }
    public Exception? Exception { get; set; }
}