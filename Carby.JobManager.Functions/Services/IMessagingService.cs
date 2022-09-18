namespace Carby.JobManager.Functions.Services;

internal interface IMessagingService
{
    public Task<IMessageProcessor> CreateProcessorAsync(string queueName);

    Task TriggerJobAsync(string jobName);
    Task TriggerNextTaskAsync(string jobName, string currentTask);
    Task TriggerFailureHandlerTaskAsync(string jobName, string currentTask);
}