using System.Diagnostics;

namespace Carby.JobManager.Functions.CarbyClient;

public interface IJobManagerClient
{
    Task StartJobAsync<T>(string jobName, T? customJobProperties);
    Activity CreateActivity(string operationName);
    void StartActivity(Activity activity);
    void StopActivity(Activity activity);
}