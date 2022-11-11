using Carby.JobManager.Functions.Attributes;
using Carby.JobManager.Functions.Tasks.AbstractCarbyTask;
using Microsoft.Azure.WebJobs.Host.Listeners;

namespace Carby.JobManager.Functions.Tasks.SplitterTask;

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