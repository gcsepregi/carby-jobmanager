namespace Carby.JobManager.Functions.JobModel;

internal sealed class JobDescriptor
{
    public string? StartTask { get; set; }
    public string? EndTask { get; set; }
    public string? HandleFailureTask { get; set; }
    public string? CleanUpTask { get; set; }
    public IDictionary<string, ICollection<TransitionDescriptor>>? Transitions { get; set; }
}