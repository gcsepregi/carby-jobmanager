namespace Carby.JobManager.Functions;

public class TaskRequest
{
    public string RequestId { get; set; }
    public BinaryData Message { get; set; }
    public long DequeueCount { get; set; }
    public DateTimeOffset? InsertedOn { get; set; }
    public DateTimeOffset? ExpiresOn { get; set; }
}