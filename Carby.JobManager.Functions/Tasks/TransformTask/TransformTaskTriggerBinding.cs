using Carby.JobManager.Functions.Attributes;
using Carby.JobManager.Functions.Tasks.AbstractCarbyTask;
using Microsoft.Azure.WebJobs.Host.Listeners;

namespace Carby.JobManager.Functions.Tasks.TransformTask;

internal sealed class TransformTaskTriggerBinding : AbstractCarbyTaskTriggerBinding<TransformTaskTriggerAttribute>
{
    public TransformTaskTriggerBinding(AbstractCarbyTaskTriggerBindingContext<TransformTaskTriggerAttribute> context) : base(context)
    {
    }

    protected override IListener CreateListener(ListenerFactoryContext context, AbstractCarbyTaskTriggerBindingContext<TransformTaskTriggerAttribute> triggerBindingContext)
    {
        return new TransformTaskListener(context, triggerBindingContext);
    }
}