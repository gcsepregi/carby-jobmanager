using System.Threading.Tasks;
using Carby.JobManager.Functions.Attributes;
using Carby.JobManager.Functions.Demo.Model;
using Carby.JobManager.Functions.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace Carby.JobManager.Functions.Demo;

public class MessageProducerDemo
{
    [FunctionName(nameof(SendMessageWhenTriggered))]
    public async Task SendMessageWhenTriggered(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "message/producer/demo")] HttpRequest req,
        [MessageProducer("message-target")] IMessageProducer messageProducer
        )
    {
        await messageProducer.SendAsync(new Message("Hello World!"));
    }
}