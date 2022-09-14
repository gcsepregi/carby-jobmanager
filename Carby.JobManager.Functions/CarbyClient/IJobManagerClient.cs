namespace Carby.JobManager.Functions;

public interface IJobManagerClient
{
    Task StartJobAsync<T>(string jobName, T? customJobProperties);
}