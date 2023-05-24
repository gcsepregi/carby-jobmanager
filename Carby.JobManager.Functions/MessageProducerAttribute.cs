using Microsoft.Azure.WebJobs.Description;

namespace Carby.JobManager.Functions;

[AttributeUsage(AttributeTargets.Parameter)]
[Binding]
public class MessageProducerAttribute : Attribute
{
    public string MessageTarget { get; set; }
}