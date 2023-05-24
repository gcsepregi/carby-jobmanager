using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace Carby.JobManager.Functions.Demo;

public class MessageProducerDemo
{
    [FunctionName(nameof(SendMessageWhenTriggeredAsync))]
    public async Task SendMessageWhenTriggeredAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "send-message-when-triggered")] HttpRequest request,
        IMessageProducer messageProducer
        )
    {
        var message = JsonSerializer.Deserialize<Dictionary<string, object>>(request.Body);
        await messageProducer.SendMessageAsync(message);
    }
}