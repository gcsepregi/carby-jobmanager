using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
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
        var parameterInfo = parameters[0];
        var customAttributes = parameterInfo.GetCustomAttributes(typeof(HttpTriggerAttribute), false);
        customAttributes.ShouldNotBeEmpty();
        customAttributes.Length.ShouldBe(1);
        var httpTriggerAttribute = ((HttpTriggerAttribute)customAttributes[0]);
        httpTriggerAttribute.AuthLevel.ShouldBe(AuthorizationLevel.Anonymous);
        httpTriggerAttribute.Methods.ShouldHaveSingleItem("post");
        httpTriggerAttribute.Route.ShouldBe("send-message-when-triggered");
        
    }
}