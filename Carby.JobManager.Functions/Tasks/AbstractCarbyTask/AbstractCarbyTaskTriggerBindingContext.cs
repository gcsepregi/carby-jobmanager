using System.Reflection;
using Carby.JobManager.Functions.Services;
using Microsoft.Azure.WebJobs.Host.Bindings;

namespace Carby.JobManager.Functions.Tasks.AbstractCarbyTask;

internal sealed class AbstractCarbyTaskTriggerBindingContext<T>
{
    public IValueBinder? ReturnValueHandler { get; set; }
    public IMessagingService? TriggerSource { get; set; }
    public T? Attribute { get; set; }
    public ParameterInfo? Parameter { get; set; }
    public MethodInfo? Method { get; set; }
    public IJobContextManagerService? JobContextManager { get; set; }
}