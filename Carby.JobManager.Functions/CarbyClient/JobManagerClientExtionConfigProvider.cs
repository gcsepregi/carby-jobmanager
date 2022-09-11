using Carby.JobManager.Functions.Attributes;
using Microsoft.Azure.WebJobs.Host.Config;

namespace Carby.JobManager.Functions.CarbyClient;

internal sealed class JobManagerClientExtionConfigProvider : IExtensionConfigProvider
{
    public void Initialize(ExtensionConfigContext context)
    {
        var bindingRule = context.AddBindingRule<JobManagerClientAttribute>();
        bindingRule.BindToInput(CreateJobManagerClient);
    }

    private IJobManagerClient CreateJobManagerClient(JobManagerClientAttribute attribute)
    {
        return new JobManagerClient();
    }
}