using Carby.JobManager.Functions.Attributes;
using Carby.JobManager.Functions.Services;
using Microsoft.Azure.WebJobs.Host.Bindings;

namespace Carby.JobManager.Functions.SimpleTask;

internal sealed class SimpleTaskTriggerBindingContext
{
    public IValueBinder? SimpleTaskReturnValueHandler { get; set; }
    public IMessagingService? TriggerSource { get; set; }
    public SimpleTaskTriggerAttribute? Attribute { get; set; }
}