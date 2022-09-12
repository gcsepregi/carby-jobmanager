using Carby.JobManager.Functions.Attributes;
using Carby.JobManager.Functions.Services;
using Microsoft.Azure.WebJobs.Host.Config;

namespace Carby.JobManager.Functions.CarbyClient;

internal sealed class JobManagerClientExtensionConfigProvider : IExtensionConfigProvider
{
    private readonly IMessagingService _messagingService;

    public JobManagerClientExtensionConfigProvider(IMessagingService messagingService)
    {
        _messagingService = messagingService;
    }
    
    public void Initialize(ExtensionConfigContext context)
    {
        var bindingRule = context.AddBindingRule<JobManagerClientAttribute>();
        bindingRule.BindToInput(CreateJobManagerClient);
    }

    private IJobManagerClient CreateJobManagerClient(JobManagerClientAttribute attribute)
    {
        return new JobManagerClient(_messagingService);
    }
}