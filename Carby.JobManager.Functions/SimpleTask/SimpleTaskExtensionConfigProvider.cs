using Carby.JobManager.Functions.AbstractCarbyTask;
using Carby.JobManager.Functions.Attributes;
using Carby.JobManager.Functions.Services;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Azure.WebJobs.Host.Triggers;

namespace Carby.JobManager.Functions.SimpleTask;

internal class SimpleTaskExtensionConfigProvider : AbstractCarbyTaskExtensionConfigProvider<SimpleTaskTriggerAttribute>
{
    public SimpleTaskExtensionConfigProvider(IMessagingService messagingProvider, IJobContextManagerService jobContextManagerService) 
        : base(messagingProvider, jobContextManagerService)
    {
    }

    protected override ITriggerBindingProvider CreateTriggerBindingProvider()
    {
        return new SimpleTaskTriggerBindingProvider(this);
    }

    protected override IValueBinder CreateReturnValueHandler()
    {
        return new SimpleTaskReturnValueHandler(JobContextManagerService);
    }

}