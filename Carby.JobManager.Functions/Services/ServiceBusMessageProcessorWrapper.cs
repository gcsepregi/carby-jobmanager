using Azure.Messaging.ServiceBus;

namespace Carby.JobManager.Functions.Services;

internal sealed class ServiceBusMessageProcessorWrapper : IMessageProcessor
{
    private readonly ServiceBusProcessor _processor;

    public ServiceBusMessageProcessorWrapper(ServiceBusProcessor processor)
    {
        _processor = processor;
    }

    public async Task StartProcessingAsync(CancellationToken cancellationToken)
    {
        await _processor.StartProcessingAsync(cancellationToken);
    }

    public async Task StopProcessingAsync(CancellationToken cancellationToken)
    {
        await _processor.StopProcessingAsync(cancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        await _processor.DisposeAsync();
    }
}