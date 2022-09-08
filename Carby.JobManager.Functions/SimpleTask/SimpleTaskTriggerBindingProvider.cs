using Microsoft.Azure.WebJobs.Host.Triggers;

namespace Carby.JobManager.Functions.SimpleTask;

public class SimpleTaskTriggerBindingProvider : ITriggerBindingProvider
{
    private readonly SimpleTaskExtensionConfigProvider _configProvider;

    public SimpleTaskTriggerBindingProvider(SimpleTaskExtensionConfigProvider configProvider)
    {
        _configProvider = configProvider;
    }

    public Task<ITriggerBinding> TryCreateAsync(TriggerBindingProviderContext context)
    {
        return Task.FromResult<ITriggerBinding>(new SimpleTaskTriggerBinding(_configProvider.CreateContext(context)));
    }
}