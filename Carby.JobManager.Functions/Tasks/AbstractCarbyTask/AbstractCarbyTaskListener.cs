using Carby.JobManager.Functions.Attributes;
using Carby.JobManager.Functions.Services;
using Microsoft.Azure.WebJobs.Host.Executors;
using Microsoft.Azure.WebJobs.Host.Listeners;

namespace Carby.JobManager.Functions.Tasks.AbstractCarbyTask;

internal abstract class AbstractCarbyTaskListener<T> : IListener where T : AbstractCarbyTaskAttribute
{
    private readonly ListenerFactoryContext _context;
    private readonly AbstractCarbyTaskTriggerBindingContext<T> _triggerBindingContext;
    private IMessageProcessor? _messageProcessor;
    
    protected AbstractCarbyTaskListener(ListenerFactoryContext context, AbstractCarbyTaskTriggerBindingContext<T> triggerBindingContext)
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
            _messageProcessor = await _triggerBindingContext.TriggerSource.CreateProcessorAsync(_triggerBindingContext.Attribute!.TaskName);
            _messageProcessor.OnMessage += ProcessMessageAsync;
            _messageProcessor.OnError += ProcessErrorAsync;
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
        await Console.Out.WriteLineAsync($"ProcessErrorAsync: {exception}");
        return false;
    }

    private async Task<MessageProcessorResult> ProcessMessageAsync(TaskRequest arg, CancellationToken cancellationToken)
    {
        var result = await _context.Executor.TryExecuteAsync(new TriggeredFunctionData
        {
            TriggerValue = arg
        }, CancellationToken.None);

        return new MessageProcessorResult
        {
            Succeeded = result.Succeeded,
            Exception = result.Exception
        };
    }
    
}