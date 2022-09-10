using System.Reflection;
using Carby.JobManager.Functions.Services;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Azure.WebJobs.Host.Config;
using Microsoft.Azure.WebJobs.Host.Triggers;

namespace Carby.JobManager.Functions.SimpleTask;

public class SimpleTaskExtensionConfigProvider : IExtensionConfigProvider
{
    private readonly IServiceBusService _serviceBus;

    public SimpleTaskExtensionConfigProvider(IServiceBusService messagingProvider)
    {
        _serviceBus = messagingProvider;
    }
    
    public void Initialize(ExtensionConfigContext context)
    {
        var triggerRule = context.AddBindingRule<SimpleTaskTriggerAttribute>();
        triggerRule.BindToTrigger(new SimpleTaskTriggerBindingProvider(this));
    }

    public SimpleTaskTriggerBindingContext CreateContext(TriggerBindingProviderContext context)
    {
        return new SimpleTaskTriggerBindingContext
        {
            TriggerSource = _serviceBus,
            SimpleTaskReturnValueHandler = CreateReturnValueHandler(),
            Attribute = context.Parameter.GetCustomAttribute<SimpleTaskTriggerAttribute>()
        };
    }

    private IValueBinder CreateReturnValueHandler()
    {
        return new SimpleTaskReturnValueHandler(this);
    }

}