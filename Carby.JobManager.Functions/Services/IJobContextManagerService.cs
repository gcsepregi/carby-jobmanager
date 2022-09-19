using Carby.JobManager.Functions.JobModel;

namespace Carby.JobManager.Functions.Services;

internal interface IJobContextManagerService
{
    Task<IJobContext> ReadJobContextAsync();
    Task PersistJobContextAsync(IJobContext jobContext);
    Task<IJobContext> ConvertUserType<T>(string jobName, T userObject);
}