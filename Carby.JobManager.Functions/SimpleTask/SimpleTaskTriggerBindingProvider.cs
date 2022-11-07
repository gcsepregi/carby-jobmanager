using Carby.JobManager.Functions.AbstractCarbyTask;
using Carby.JobManager.Functions.Attributes;
using Microsoft.Azure.WebJobs.Host.Triggers;

namespace Carby.JobManager.Functions.SimpleTask;

internal sealed class SimpleTaskTriggerBindingProvider : AbstractCarbyTaskTriggerBindingProvider<SimpleTaskTriggerAttribute>
{
    public SimpleTaskTriggerBindingProvider(AbstractCarbyTaskExtensionConfigProvider<SimpleTaskTriggerAttribute> configProvider) : base(configProvider)
    {
    }

    protected override ITriggerBinding CreateTriggerBinding(AbstractCarbyTaskTriggerBindingContext<SimpleTaskTriggerAttribute> context)
    {
        return new SimpleTaskTriggerBinding(context);
    }
}