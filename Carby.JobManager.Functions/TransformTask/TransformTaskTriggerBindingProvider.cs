using Carby.JobManager.Functions.AbstractCarbyTask;
using Carby.JobManager.Functions.Attributes;
using Microsoft.Azure.WebJobs.Host.Triggers;

namespace Carby.JobManager.Functions.TransformTask;

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