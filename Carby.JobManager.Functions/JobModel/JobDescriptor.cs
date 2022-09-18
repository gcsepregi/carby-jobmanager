namespace Carby.JobManager.Functions.JobModel;

internal sealed class JobDescriptor
{
    public string StartTask { get; set; } = null!;
    public string EndTask { get; set; } = null!;
    public string HandleFailureTask { get; set; } = null!;
    public string CleanUpTask { get; set; } = null!;
    public IDictionary<string, ICollection<TransitionDescriptor>> Transitions { get; set; } =
        new Dictionary<string, ICollection<TransitionDescriptor>>();
}