using Microsoft.Azure.WebJobs.Description;

namespace Carby.JobManager.Functions.Attributes;

[AttributeUsage(AttributeTargets.Parameter)]
[Binding]
public class JobManagerClientAttribute : Attribute
{
    
}