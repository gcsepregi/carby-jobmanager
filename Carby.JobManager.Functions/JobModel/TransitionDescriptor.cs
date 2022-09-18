namespace Carby.JobManager.Functions.JobModel;

internal sealed class TransitionDescriptor
{
    public string? FromTask { get; set; }
    public string? ToTask { get; set; }
    public Func<IJobContext, bool>? When { get; set; }
}