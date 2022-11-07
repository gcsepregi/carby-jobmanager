using Microsoft.Azure.WebJobs.Description;

namespace Carby.JobManager.Functions.Attributes;

[AttributeUsage(AttributeTargets.Parameter)]
#pragma warning disable CS0618
[Binding(TriggerHandlesReturnValue = true)]
#pragma warning restore CS0618
public class SplitterTaskTriggerAttribute : AbstractCarbyTaskAttribute
{
    public SplitterTaskTriggerAttribute(string taskName) : base(taskName)
    {
    }
}