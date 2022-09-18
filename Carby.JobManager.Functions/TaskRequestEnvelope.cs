namespace Carby.JobManager.Functions;

public class TaskRequestEnvelope
{
    public TaskRequest Request { get; set; } = new TaskRequest();
    public IDictionary<string, string> Headers { get; set; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
}