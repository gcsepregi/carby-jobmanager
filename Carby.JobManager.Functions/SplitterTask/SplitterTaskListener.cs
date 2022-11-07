using Carby.JobManager.Functions.AbstractCarbyTask;
using Carby.JobManager.Functions.Attributes;
using Microsoft.Azure.WebJobs.Host.Listeners;

namespace Carby.JobManager.Functions.SplitterTask;

internal class SplitterTaskListener : AbstractCarbyTaskListener<SplitterTaskTriggerAttribute> {
    public SplitterTaskListener(ListenerFactoryContext context, AbstractCarbyTaskTriggerBindingContext<SplitterTaskTriggerAttribute> triggerBindingContext) : base(context, triggerBindingContext)
    {
    }
}