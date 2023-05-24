namespace Carby.JobManager.Functions;

public interface IMessageProducer
{
    Task SendMessageAsync(object message);
}