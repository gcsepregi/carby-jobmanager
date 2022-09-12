namespace Carby.JobManager.Functions;

public interface IJobManagerClient
{
    Task StartJobAsync(string jobName);
}