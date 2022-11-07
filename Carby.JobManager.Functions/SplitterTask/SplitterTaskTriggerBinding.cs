using Carby.JobManager.Functions.AbstractCarbyTask;
using Carby.JobManager.Functions.Attributes;
using Microsoft.Azure.WebJobs.Host.Listeners;

namespace Carby.JobManager.Functions.SplitterTask;

internal class SplitterTaskTriggerBinding : AbstractCarbyTaskTriggerBinding<SplitterTaskTriggerAttribute>
{
    private readonly AbstractCarbyTaskTriggerBindingContext<SplitterTaskTriggerAttribute> _context;

    public SplitterTaskTriggerBinding(AbstractCarbyTaskTriggerBindingContext<SplitterTaskTriggerAttribute> context) : base(context)
    {
    }

    protected override IListener CreateListener(ListenerFactoryContext context, AbstractCarbyTaskTriggerBindingContext<SplitterTaskTriggerAttribute> triggerBindingContext)
    {
        return new SplitterTaskListener(context, triggerBindingContext);
    }
}