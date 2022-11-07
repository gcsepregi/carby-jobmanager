namespace Carby.JobManager.Functions.Attributes;

public abstract class AbstractCarbyTaskAttribute :  Attribute
{
    public string TaskName { get; }
    
    public string? JobName { get; set; }

    protected AbstractCarbyTaskAttribute(string taskName)
    {
        TaskName = taskName;
    }
}