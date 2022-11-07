using System.Reflection;
using Carby.JobManager.Functions.Services;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Azure.WebJobs.Host.Config;
using Microsoft.Azure.WebJobs.Host.Triggers;

namespace Carby.JobManager.Functions.AbstractCarbyTask;

internal abstract class AbstractCarbyTaskExtensionConfigProvider<T> : IExtensionConfigProvider where T : Attribute
{
    public IMessagingService MessagingProvider { get; }
    public IJobContextManagerService JobContextManagerService { get; }

    protected AbstractCarbyTaskExtensionConfigProvider(IMessagingService messagingProvider, IJobContextManagerService jobContextManagerService)
    {
        MessagingProvider = messagingProvider;
        JobContextManagerService = jobContextManagerService;
    }

    public void Initialize(ExtensionConfigContext context)
    {
        var triggerRule = context.AddBindingRule<T>();
        triggerRule.BindToTrigger(CreateTriggerBindingProvider());
    }

    public AbstractCarbyTaskTriggerBindingContext<T> CreateContext(TriggerBindingProviderContext context)
    {
        return new AbstractCarbyTaskTriggerBindingContext<T>
        {
            TriggerSource = MessagingProvider,
            ReturnValueHandler = CreateReturnValueHandler(),
            Attribute = context.Parameter.GetCustomAttribute<T>(),
            Parameter = context.Parameter,
            Method = (MethodInfo)context.Parameter.Member,
            JobContextManager = JobContextManagerService
        };
    }

    protected abstract IValueBinder CreateReturnValueHandler();
    protected abstract ITriggerBindingProvider CreateTriggerBindingProvider();
}