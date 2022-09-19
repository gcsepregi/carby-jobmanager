using System.Diagnostics;
using Carby.JobManager.Functions.Services;
using Carby.JobManager.Functions.Tracing;

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

        var activity = jobName.CreateActivity(ActivityKind.Internal);
        activity.AddBaggage(ICommonServices.InternalJobIdKey, jobContext.JobId);
        activity.AddBaggage(ICommonServices.CurrentJobNameKey, jobName);
        activity.AddTag(ICommonServices.TaskInstanceIdKey, ActivityTraceId.CreateRandom().ToHexString());
        activity.StartActivity();

        await _jobContextManagerService.PersistJobContextAsync(jobContext);
        await _messagingService.TriggerJobAsync(jobName);
        
        activity.StopActivity();
    }

    public Activity CreateActivity(string operationName)
    {
        var activity = operationName.CreateActivity(ActivityKind.Client);
        activity.AddBaggage(ICommonServices.CurrentJobNameKey, operationName);
        activity.AddBaggage(ICommonServices.InternalJobIdKey, Guid.NewGuid().ToString("D"));
        activity.AddTag(ICommonServices.TaskInstanceIdKey, ActivityTraceId.CreateRandom().ToHexString());
        return activity;
    }

    public void StartActivity(Activity activity)
    {
        activity.StartActivity();
    }

    public void StopActivity(Activity activity)
    {
        activity.StopActivity();
    }
}