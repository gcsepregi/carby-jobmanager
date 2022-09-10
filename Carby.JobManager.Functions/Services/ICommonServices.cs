namespace Carby.JobManager.Functions.Services;

public interface ICommonServices
{
    string GetJobName();

    static readonly string DefaultJobName = "CarbyDefaultJob";
}