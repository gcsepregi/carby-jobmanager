namespace Carby.JobManager.Functions.Services;

public interface IMessageProcessor : IAsyncDisposable
{
    Task StartProcessingAsync(CancellationToken cancellationToken);
    Task StopProcessingAsync(CancellationToken cancellationToken = default);
}