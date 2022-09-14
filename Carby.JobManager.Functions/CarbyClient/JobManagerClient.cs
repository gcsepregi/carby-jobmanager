using System.Diagnostics;
using Carby.JobManager.Functions.JobModel;
using Carby.JobManager.Functions.Services;

namespace Carby.JobManager.Functions.CarbyClient;

internal sealed class JobManagerClient : IJobManagerClient
{
    private readonly IMessagingService _messagingService;
    private readonly IJobContextManagerService _jobContextManagerService;

    public JobManagerClient(
        IMessagingService messagingService, 
        IJobContextManagerService jobContextManagerService
        )
    {
        _messagingService = messagingService;
        _jobContextManagerService = jobContextManagerService;
    }

    public async Task StartJobAsync<T>(string jobName, T? customJobProperties)
    {
        Console.WriteLine($"Starting job named {jobName}");
        var jobContext = await _jobContextManagerService.ConvertUserType(jobName, customJobProperties);
        SetUpActivity<T>(jobContext);
        await _jobContextManagerService.PersistJobContextAsync(jobName, jobContext);
        await _messagingService.TriggerJobAsync(jobName);
    }

    private static void SetUpActivity<T>(IJobContext jobContext)
    {
        Activity.Current?.AddBaggage("InternalJobId", jobContext.JobId);
    }
}