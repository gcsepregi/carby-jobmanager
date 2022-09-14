namespace Carby.JobManager.Functions.JobModel;

public interface IJobContext : IDictionary<string, object>
{
    public string JobId { get; set; }
}