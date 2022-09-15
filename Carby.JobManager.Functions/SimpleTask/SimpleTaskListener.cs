using Carby.JobManager.Functions.Services;
using Microsoft.Azure.WebJobs.Host.Executors;
using Microsoft.Azure.WebJobs.Host.Listeners;

namespace Carby.JobManager.Functions.SimpleTask;

internal sealed class SimpleTaskListener : IListener
{
    private readonly ListenerFactoryContext _context;
    private readonly SimpleTaskTriggerBindingContext _triggerBindingContext;
    private IMessageProcessor? _serviceBusProcessor;

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

    private async Task ProcessErrorAsync(Exception exception)
    {
        await Console.Error.WriteLineAsync(exception.ToString());
    }

    private async Task<MessageProcessorResult> ProcessMessageAsync(TaskRequest arg, CancellationToken cancellationToken)
    {
        var result = await _context.Executor.TryExecuteAsync(new TriggeredFunctionData
        {
            TriggerValue = new TaskRequest()
        }, CancellationToken.None);

        return new MessageProcessorResult
        {
            Succeeded = result.Succeeded,
            Exception = result.Exception
        };
    }

}