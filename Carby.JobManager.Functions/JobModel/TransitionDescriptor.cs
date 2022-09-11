namespace Carby.JobManager.Functions.JobModel;

internal sealed class TransitionDescriptor
{
    public string? FromTask { get; set; }
    public string? ToTask { get; set; }
    public Func<JobContext, bool>? When { get; set; }
}