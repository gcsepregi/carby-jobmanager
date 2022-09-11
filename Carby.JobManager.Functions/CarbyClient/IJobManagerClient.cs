namespace Carby.JobManager.Functions;

public interface IJobManagerClient
{
    void StartJob(string jobName);
}