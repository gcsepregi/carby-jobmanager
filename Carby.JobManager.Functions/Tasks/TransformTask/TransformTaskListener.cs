using Carby.JobManager.Functions.Attributes;
using Carby.JobManager.Functions.Tasks.AbstractCarbyTask;
using Microsoft.Azure.WebJobs.Host.Listeners;

namespace Carby.JobManager.Functions.Tasks.TransformTask;

internal sealed class TransformTaskListener : AbstractCarbyTaskListener<TransformTaskTriggerAttribute>
{
    public TransformTaskListener(ListenerFactoryContext context, AbstractCarbyTaskTriggerBindingContext<TransformTaskTriggerAttribute> triggerBindingContext)
    : base(context, triggerBindingContext)
    {
    }

}