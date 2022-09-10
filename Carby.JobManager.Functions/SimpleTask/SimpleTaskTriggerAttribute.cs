using Microsoft.Azure.WebJobs.Description;

namespace Carby.JobManager.Functions.SimpleTask;

[AttributeUsage(AttributeTargets.Parameter)]
#pragma warning disable CS0618
[Binding(TriggerHandlesReturnValue = true)]
#pragma warning restore CS0618
public class SimpleTaskTriggerAttribute : Attribute
{
    public string TaskName { get; }
    
    public string? JobName { get; set; }

    public SimpleTaskTriggerAttribute(string taskName)
    {
        TaskName = taskName;
    }
}