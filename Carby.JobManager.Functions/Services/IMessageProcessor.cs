namespace Carby.JobManager.Functions.Services;

internal interface IMessageProcessor : IAsyncDisposable
{
    public event Func<TaskRequest, CancellationToken, Task<MessageProcessorResult>>? OnMessage;
    public event Func<Exception, Task<bool>>? OnError;
    
    Task StartProcessingAsync(CancellationToken cancellationToken);
    Task StopProcessingAsync(CancellationToken cancellationToken = default);
}