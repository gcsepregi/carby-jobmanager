using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using NSubstitute;
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
    
    [Test]
    public void SendMessageWhenTriggered_HasProperHttpRequestAsFirstParameter()
    {
        var messageProducerDemo = new MessageProducerDemo();
        var methodInfo = messageProducerDemo.GetType().GetMethod(nameof(MessageProducerDemo.SendMessageWhenTriggeredAsync));
        methodInfo.ShouldNotBeNull();
        var parameters = methodInfo.GetParameters();
        parameters.ShouldNotBeEmpty();
        var parameterInfo = parameters[0];
        parameterInfo.ParameterType.ShouldBe(typeof(HttpRequest));
    }
    
    [Test]
    public void SendMessageWhenTriggered_HasMessageProducerAsSecondArgument()
    {
        var messageProducerDemo = new MessageProducerDemo();
        var methodInfo = messageProducerDemo.GetType().GetMethod(nameof(MessageProducerDemo.SendMessageWhenTriggeredAsync));
        methodInfo.ShouldNotBeNull();
        var parameters = methodInfo.GetParameters();
        parameters.ShouldNotBeEmpty();
        parameters.Length.ShouldBe(2);
        var parameterInfo = parameters[1];
        parameterInfo.ParameterType.ShouldBe(typeof(IMessageProducer));
    }
    
    [Test]
    public async Task SendMessageWhenTriggered_CallsMessageProducer_SendMessageAsync()
    {
        var messageProducerDemo = new MessageProducerDemo();
        var messageProducer = Substitute.For<IMessageProducer>();
        var request = Substitute.For<HttpRequest>();
        
        request.Body.Returns(new MemoryStream("{\"hello\":\"from test\"}".Select(c => (byte)c).ToArray()));
        
        await messageProducerDemo.SendMessageWhenTriggeredAsync(request, messageProducer);
        
        await messageProducer.Received(1).SendMessageAsync(
            Arg.Is<Dictionary<string, object>>(
                p => p.ContainsKey("hello") && p["hello"].ToString() == "from test"
                )
            );
    }
    
    [Test]
    public void SendMessageWhenTriggered_HasProperMessageProducerAttribute()
    {
        var messageProducerDemo = new MessageProducerDemo();
        var methodInfo = messageProducerDemo.GetType().GetMethod(nameof(MessageProducerDemo.SendMessageWhenTriggeredAsync));
        methodInfo.ShouldNotBeNull();
        var parameters = methodInfo.GetParameters();
        parameters.ShouldNotBeEmpty();
        parameters.Length.ShouldBe(2);
        var parameterInfo = parameters[1];
        var customAttributes = parameterInfo.GetCustomAttributes(typeof(MessageProducerAttribute), false);
        customAttributes.ShouldNotBeEmpty();
        customAttributes.Length.ShouldBe(1);
        var messageProducerAttribute = ((MessageProducerAttribute)customAttributes[0]);
        messageProducerAttribute.MessageTarget.ShouldBe("message-target");
    }
}