namespace Carby.JobManager.Functions.CarbyClient;

internal sealed class JobManagerClient : IJobManagerClient
{
    public void StartJob(string jobName)
    {
        Console.WriteLine($"Starting job named {jobName}");
    }
}