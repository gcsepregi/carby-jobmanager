using Carby.JobManager.Functions.AbstractCarbyTask;
using Carby.JobManager.Functions.Attributes;
using Microsoft.Azure.WebJobs.Host.Listeners;

namespace Carby.JobManager.Functions.SimpleTask;

internal sealed class SimpleTaskTriggerBinding : AbstractCarbyTaskTriggerBinding<SimpleTaskTriggerAttribute>
{
    private readonly AbstractCarbyTaskTriggerBindingContext<SimpleTaskTriggerAttribute> _context;

    public SimpleTaskTriggerBinding(AbstractCarbyTaskTriggerBindingContext<SimpleTaskTriggerAttribute> context) : base(context)
    {
    }

    protected override IListener CreateListener(ListenerFactoryContext context, AbstractCarbyTaskTriggerBindingContext<SimpleTaskTriggerAttribute> triggerBindingContext)
    {
        return new SimpleTaskListener(context, triggerBindingContext);
    }
}