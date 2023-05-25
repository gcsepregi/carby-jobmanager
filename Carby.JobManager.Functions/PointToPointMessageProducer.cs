
using System.Text.Json;

namespace Carby.JobManager.Functions;

public class PointToPointMessageProducer : IMessageProducer
{
    private readonly string _messageTarget;

    public PointToPointMessageProducer(string messageTarget)
    {
        _messageTarget = messageTarget;
    }

    public Task SendMessageAsync(object message)
    {
        Console.WriteLine($"Sending message to channel {_messageTarget} : {JsonSerializer.Serialize(message)}");
        return Task.CompletedTask;
    }
}