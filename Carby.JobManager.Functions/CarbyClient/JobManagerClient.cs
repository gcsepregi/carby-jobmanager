using Carby.JobManager.Functions.Services;

namespace Carby.JobManager.Functions.CarbyClient;

internal sealed class JobManagerClient : IJobManagerClient
{
    private readonly IMessagingService _messagingService;

    public JobManagerClient(IMessagingService messagingService)
    {
        _messagingService = messagingService;
    }

    public async Task StartJobAsync(string? jobName)
    {
        Console.WriteLine($"Starting job named {jobName}");
        await _messagingService.TriggerJobAsync(jobName);
    }
}