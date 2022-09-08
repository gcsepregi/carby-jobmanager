using Microsoft.Azure.WebJobs.Host.Config;
using Microsoft.Azure.WebJobs.Host.Triggers;

namespace Carby.JobManager.Functions.SimpleTask;

public class SimpleTaskExtensionConfigProvider : IExtensionConfigProvider
{
    public void Initialize(ExtensionConfigContext context)
    {
        var triggerRule = context.AddBindingRule<SimpleTaskTriggerAttribute>();
        triggerRule.BindToTrigger(new SimpleTaskTriggerBindingProvider(this));
    }

    public SimpleTaskTriggerBindingContext CreateContext(TriggerBindingProviderContext context)
    {
        throw new NotImplementedException();
    }
}