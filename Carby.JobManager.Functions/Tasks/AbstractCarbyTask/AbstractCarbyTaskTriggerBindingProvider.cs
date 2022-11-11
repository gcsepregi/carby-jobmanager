using System.Reflection;
using Microsoft.Azure.WebJobs.Host.Triggers;

namespace Carby.JobManager.Functions.Tasks.AbstractCarbyTask;

internal abstract class AbstractCarbyTaskTriggerBindingProvider<T> : ITriggerBindingProvider where T : Attribute
{
    private readonly AbstractCarbyTaskExtensionConfigProvider<T> _configProvider;

    public AbstractCarbyTaskTriggerBindingProvider(AbstractCarbyTaskExtensionConfigProvider<T> configProvider)
    {
        _configProvider = configProvider;
    }

    public Task<ITriggerBinding> TryCreateAsync(TriggerBindingProviderContext context)
    {
        var parameter = context.Parameter;
        var attribute = parameter.GetCustomAttribute<T>(false);
        if (attribute == null)
        {
            return Task.FromResult<ITriggerBinding>(null!);
        }

        return Task.FromResult(CreateTriggerBinding(_configProvider.CreateContext(context)));
    }

    protected abstract ITriggerBinding CreateTriggerBinding(AbstractCarbyTaskTriggerBindingContext<T> context);
}