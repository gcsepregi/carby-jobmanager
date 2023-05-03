namespace Carby.JobManager.Functions.Demo.Model;

public class Message
{
    public string MessageText { get; init; }
    
    public Message(string message)
    {
        MessageText = message;
    }

}