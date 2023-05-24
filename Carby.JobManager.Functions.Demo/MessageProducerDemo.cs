using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;

namespace Carby.JobManager.Functions.Demo;

public class MessageProducerDemo
{
    [FunctionName(nameof(SendMessageWhenTriggeredAsync))]
    public async Task SendMessageWhenTriggeredAsync(object arg1)
    {
        
    }
}