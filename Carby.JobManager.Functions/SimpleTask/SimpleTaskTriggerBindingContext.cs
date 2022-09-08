using Microsoft.Azure.WebJobs.Host.Bindings;

namespace Carby.JobManager.Functions.SimpleTask;

public class SimpleTaskTriggerBindingContext
{
    public IValueBinder SimpleTaskReturnValueHandler { get; set; }
}