using Azure.Messaging.ServiceBus;
using Microsoft.Azure.WebJobs.Host.Executors;
using Microsoft.Azure.WebJobs.Host.Listeners;

namespace Carby.JobManager.Functions.SimpleTask;

public class SimpleTaskListener : IListener
{
    private readonly ListenerFactoryContext _context;
    private readonly SimpleTaskTriggerBindingContext _triggerBindingContext;
    private ServiceBusProcessor? _serviceBusProcessor;

    public SimpleTaskListener(ListenerFactoryContext context, SimpleTaskTriggerBindingContext triggerBindingContext)
    {
        _context = context;
        _triggerBindingContext = triggerBindingContext;
    }

    public void Dispose()
    {
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (_triggerBindingContext.TriggerSource != null)
        {
            _serviceBusProcessor = await _triggerBindingContext.TriggerSource.CreateProcessorAsync(
                _triggerBindingContext.Attribute!.JobName,
                _triggerBindingContext.Attribute!.TaskName.ToLowerInvariant(),
                ProcessMessageAsync, 
                ProcessErrorAsync);
            await _serviceBusProcessor.StartProcessingAsync(cancellationToken);
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_serviceBusProcessor != null)
        {
            await _serviceBusProcessor.StopProcessingAsync(cancellationToken);
        }
    }

    public void Cancel()
    {
        Task.Run(async () =>
        {
            if (_serviceBusProcessor != null)
            {
                await _serviceBusProcessor.StopProcessingAsync();
            }
        });
    }

    private async Task ProcessErrorAsync(ProcessErrorEventArgs arg)
    {
        await Console.Error.WriteLineAsync(arg.Exception.ToString());
    }

    private async Task ProcessMessageAsync(ProcessMessageEventArgs arg)
    {
        await _context.Executor.TryExecuteAsync(new TriggeredFunctionData
        {
            TriggerValue = new TaskRequest()
        }, CancellationToken.None);
    }

}