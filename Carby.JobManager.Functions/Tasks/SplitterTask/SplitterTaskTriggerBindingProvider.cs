using Carby.JobManager.Functions.Attributes;
using Carby.JobManager.Functions.Tasks.AbstractCarbyTask;
using Microsoft.Azure.WebJobs.Host.Triggers;

namespace Carby.JobManager.Functions.Tasks.SplitterTask;

internal class SplitterTaskTriggerBindingProvider : AbstractCarbyTaskTriggerBindingProvider<SplitterTaskTriggerAttribute>
{

    public SplitterTaskTriggerBindingProvider(AbstractCarbyTaskExtensionConfigProvider<SplitterTaskTriggerAttribute> configProvider) : base(configProvider)
    {
    }

    protected override ITriggerBinding CreateTriggerBinding(AbstractCarbyTaskTriggerBindingContext<SplitterTaskTriggerAttribute> context)
    {
        return new SplitterTaskTriggerBinding(context);
    }
}