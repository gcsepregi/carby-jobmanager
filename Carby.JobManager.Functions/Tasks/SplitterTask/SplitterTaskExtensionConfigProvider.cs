using Carby.JobManager.Functions.Attributes;
using Carby.JobManager.Functions.Services;
using Carby.JobManager.Functions.Tasks.AbstractCarbyTask;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Azure.WebJobs.Host.Triggers;

namespace Carby.JobManager.Functions.Tasks.SplitterTask;

internal class SplitterTaskExtensionConfigProvider : AbstractCarbyTaskExtensionConfigProvider<SplitterTaskTriggerAttribute>
{
    public SplitterTaskExtensionConfigProvider(IMessagingService messagingProvider, IJobContextManagerService jobContextManagerService) 
        : base(messagingProvider, jobContextManagerService)
    {
    }

    protected override ITriggerBindingProvider CreateTriggerBindingProvider()
    {
        return new SplitterTaskTriggerBindingProvider(this);
    }

    protected override IValueBinder CreateReturnValueHandler()
    {
        return new SplitterTaskReturnValueHandler(JobContextManagerService);
    }

}