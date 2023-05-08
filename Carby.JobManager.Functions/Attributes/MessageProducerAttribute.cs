using Microsoft.Azure.WebJobs.Description;

namespace Carby.JobManager.Functions.Attributes;

[AttributeUsage( AttributeTargets.Parameter)]
[Binding]
public class MessageProducerAttribute : Attribute
{
    public string Target { get; }
    
    public MessageProducerAttribute(string target)
    {
        Target = target;
    }

}