using System.Reflection;
using Carby.JobManager.Functions.Attributes;
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
        var parameter = context.Parameter;
        var attribute = parameter.GetCustomAttribute<SimpleTaskTriggerAttribute>(false);
        if (attribute == null)
        {
            return Task.FromResult<ITriggerBinding>(null!);
        }

        return Task.FromResult<ITriggerBinding>(new SimpleTaskTriggerBinding(_configProvider.CreateContext(context)));
    }
}