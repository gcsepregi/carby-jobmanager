using Carby.JobManager.Functions.AbstractCarbyTask;
using Carby.JobManager.Functions.Attributes;
using Microsoft.Azure.WebJobs.Host.Listeners;

namespace Carby.JobManager.Functions.SimpleTask;

internal sealed class SimpleTaskListener : AbstractCarbyTaskListener<SimpleTaskTriggerAttribute>
{
    public SimpleTaskListener(ListenerFactoryContext context, AbstractCarbyTaskTriggerBindingContext<SimpleTaskTriggerAttribute> triggerBindingContext)
    : base(context, triggerBindingContext)
    {
    }

}