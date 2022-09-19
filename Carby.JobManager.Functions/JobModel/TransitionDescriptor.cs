namespace Carby.JobManager.Functions.JobModel;

internal sealed class TransitionDescriptor
{
    public string FromTask { get; set; } = null!;
    public string ToTask { get; set; } = null!;
    public Func<IJobContext, bool>? When { get; set; }
    
    public Func<IJobContext, int>? FanOut { get; set; }
    
    public bool FanIn { get; set; }
}