using Carby.JobManager.Functions.Services;
using Microsoft.Azure.WebJobs.Host.Bindings;

namespace Carby.JobManager.Functions.SimpleTask;

public class SimpleTaskTriggerBindingContext
{
    public IValueBinder? SimpleTaskReturnValueHandler { get; set; }
    public IServiceBusService? TriggerSource { get; set; }
    public SimpleTaskTriggerAttribute? Attribute { get; set; }
}