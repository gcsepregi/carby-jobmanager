using NUnit.Framework;

namespace Carby.JobManager.Functions.Demo.Tests;

[TestFixture]
public class MessagingProducerDemoTests
{

    [Test]
    public async Task Test()
    {
        var messageProducerDemo = new MessageProducerDemo();
        var request = new HttpRequest();
        await messageProducerDemo.SendMessageWhenTriggeredAsync(request);
    }
    
}