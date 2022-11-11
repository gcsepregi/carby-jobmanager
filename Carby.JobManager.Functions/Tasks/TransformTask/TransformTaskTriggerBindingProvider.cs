using Carby.JobManager.Functions.Attributes;
using Carby.JobManager.Functions.Tasks.AbstractCarbyTask;
using Microsoft.Azure.WebJobs.Host.Triggers;

namespace Carby.JobManager.Functions.Tasks.TransformTask;

internal sealed class TransformTaskTriggerBindingProvider : AbstractCarbyTaskTriggerBindingProvider<TransformTaskTriggerAttribute>
{
    public TransformTaskTriggerBindingProvider(AbstractCarbyTaskExtensionConfigProvider<TransformTaskTriggerAttribute> configProvider) : base(configProvider)
    {
    }

    protected override ITriggerBinding CreateTriggerBinding(AbstractCarbyTaskTriggerBindingContext<TransformTaskTriggerAttribute> context)
    {
        return new TransformTaskTriggerBinding(context);
    }
}