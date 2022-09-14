using Carby.JobManager.Functions.JobModel;

namespace Carby.JobManager.Functions.Services;

internal interface IJobContextManagerService
{
    Task<IJobContext> ReadJobContextAsync(string? jobName);
    Task PersistJobContextAsync(string? jobName, IJobContext jobContext);
    Task<IJobContext> ConvertUserType<T>(string  jobName, T userObject);
}