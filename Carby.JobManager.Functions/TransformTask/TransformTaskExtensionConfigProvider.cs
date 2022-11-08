using Carby.JobManager.Functions.AbstractCarbyTask;
using Carby.JobManager.Functions.Attributes;
using Carby.JobManager.Functions.Services;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Azure.WebJobs.Host.Triggers;

namespace Carby.JobManager.Functions.TransformTask;

internal class TransformTaskExtensionConfigProvider : AbstractCarbyTaskExtensionConfigProvider<TransformTaskTriggerAttribute>
{
    public TransformTaskExtensionConfigProvider(IMessagingService messagingProvider, IJobContextManagerService jobContextManagerService) 
        : base(messagingProvider, jobContextManagerService)
    {
    }

    protected override ITriggerBindingProvider CreateTriggerBindingProvider()
    {
        return new TransformTaskTriggerBindingProvider(this);
    }

    protected override IValueBinder CreateReturnValueHandler()
    {
        return new TransformTaskReturnValueHandler(JobContextManagerService);
    }

}