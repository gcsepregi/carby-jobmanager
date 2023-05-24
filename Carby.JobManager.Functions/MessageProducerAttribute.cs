namespace Carby.JobManager.Functions;

public class MessageProducerAttribute : Attribute
{
    public string MessageTarget { get; set; }
}