using Carby.JobManager.Functions.Attributes;
using Carby.JobManager.Functions.Tasks.AbstractCarbyTask;
using Microsoft.Azure.WebJobs.Host.Listeners;

namespace Carby.JobManager.Functions.Tasks.SplitterTask;

internal class SplitterTaskListener : AbstractCarbyTaskListener<SplitterTaskTriggerAttribute> {
    public SplitterTaskListener(ListenerFactoryContext context, AbstractCarbyTaskTriggerBindingContext<SplitterTaskTriggerAttribute> triggerBindingContext) : base(context, triggerBindingContext)
    {
    }
}