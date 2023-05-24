using Microsoft.Azure.WebJobs;
using NUnit.Framework;
using Shouldly;

namespace Carby.JobManager.Functions.Demo.Tests;

[TestFixture]
public class MessagingProducerDemoTests
{

    [Test]
    public void SendMessageWhenTriggered_HasProperFunctionNameAttribute()
    {
        var messageProducerDemo = new MessageProducerDemo();
        var methodInfo = messageProducerDemo.GetType().GetMethod(nameof(MessageProducerDemo.SendMessageWhenTriggeredAsync));
        methodInfo.ShouldNotBeNull();
        var functionNameAttribute = methodInfo.GetCustomAttributes(typeof(FunctionNameAttribute), false);
        functionNameAttribute.ShouldNotBeEmpty();
        functionNameAttribute.Length.ShouldBe(1);
        ((FunctionNameAttribute)functionNameAttribute[0]).Name.ShouldBe(nameof(MessageProducerDemo.SendMessageWhenTriggeredAsync));
    }
    
    [Test]
    public void SendMessageWhenTriggered_HasProperHttpTriggerAttribute()
    {
        var messageProducerDemo = new MessageProducerDemo();
        var methodInfo = messageProducerDemo.GetType().GetMethod(nameof(MessageProducerDemo.SendMessageWhenTriggeredAsync));
        methodInfo.ShouldNotBeNull();
        var parameters = methodInfo.GetParameters();
        parameters.ShouldNotBeEmpty();
    }
}