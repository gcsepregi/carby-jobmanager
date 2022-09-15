using Carby.JobManager.Functions.Services;
using Microsoft.Azure.WebJobs.Host.Executors;
using Microsoft.Azure.WebJobs.Host.Listeners;

namespace Carby.JobManager.Functions.SimpleTask;

internal sealed class SimpleTaskListener : IListener
{
    private readonly ListenerFactoryContext _context;
    private readonly SimpleTaskTriggerBindingContext _triggerBindingContext;
    private IMessageProcessor? _messageProcessor;

    public SimpleTaskListener(ListenerFactoryContext context, SimpleTaskTriggerBindingContext triggerBindingContext)
    {
        _context = context;
        _triggerBindingContext = triggerBindingContext;
    }

    public void Dispose()
    {
        if (_messageProcessor != null)
        {
            Task.Run(async () => await _messageProcessor.DisposeAsync());
        }
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (_triggerBindingContext.TriggerSource != null)
        {
            _messageProcessor = await _triggerBindingContext.TriggerSource.CreateProcessorAsync(
                _triggerBindingContext.Attribute!.JobName,
                _triggerBindingContext.Attribute!.TaskName,
                ProcessMessageAsync, 
                ProcessErrorAsync);
            await _messageProcessor.StartProcessingAsync(cancellationToken);
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_messageProcessor != null)
        {
            await _messageProcessor.StopProcessingAsync(cancellationToken);
        }
    }

    public void Cancel()
    {
        Task.Run(async () =>
        {
            if (_messageProcessor != null)
            {
                await _messageProcessor.StopProcessingAsync();
            }
        });
    }

    private async Task<bool> ProcessErrorAsync(Exception exception)
    {
        await Console.Out.WriteLineAsync($"ProcessErrorAsync: {exception.Message}");
        return false;
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