using Carby.JobManager.Functions.Configurations;

namespace Carby.JobManager.Functions.Interfaces;

public interface IMessageProducer
{
    public Task SendAsync(object message, MessageProducerOptions? options = default);
}