using Microsoft.Azure.WebJobs.Host.Listeners;

namespace Carby.JobManager.Functions.SimpleTask;

public class SimpleTaskListener : IListener
{
    private readonly ListenerFactoryContext _context;
    private readonly SimpleTaskTriggerBindingContext _triggerBindingContext;

    public SimpleTaskListener(ListenerFactoryContext context, SimpleTaskTriggerBindingContext triggerBindingContext)
    {
        _context = context;
        _triggerBindingContext = triggerBindingContext;
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public void Cancel()
    {
        throw new NotImplementedException();
    }
}