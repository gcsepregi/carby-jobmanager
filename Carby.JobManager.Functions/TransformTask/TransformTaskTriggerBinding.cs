using Carby.JobManager.Functions.AbstractCarbyTask;
using Carby.JobManager.Functions.Attributes;
using Microsoft.Azure.WebJobs.Host.Listeners;

namespace Carby.JobManager.Functions.TransformTask;

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