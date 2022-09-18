namespace Carby.JobManager.Functions.Services;

public interface ICommonServices
{
    const string DefaultJobName = "CarbyDefaultJob";
    const string CurrentJobNameKey = "CurrentJobName";
    const string InternalJobIdKey = "InternalJobId";
    const string TaskInstanceIdKey = "TaskInstanceId";
    const string CurrentTaskNameKey = "CurrentTaskName";
}