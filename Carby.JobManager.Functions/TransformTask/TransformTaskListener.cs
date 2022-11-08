using Carby.JobManager.Functions.AbstractCarbyTask;
using Carby.JobManager.Functions.Attributes;
using Microsoft.Azure.WebJobs.Host.Listeners;

namespace Carby.JobManager.Functions.TransformTask;

internal sealed class TransformTaskListener : AbstractCarbyTaskListener<TransformTaskTriggerAttribute>
{
    public TransformTaskListener(ListenerFactoryContext context, AbstractCarbyTaskTriggerBindingContext<TransformTaskTriggerAttribute> triggerBindingContext)
    : base(context, triggerBindingContext)
    {
    }

}