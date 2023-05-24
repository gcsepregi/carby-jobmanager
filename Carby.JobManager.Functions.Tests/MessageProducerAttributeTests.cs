using System.Reflection;
using Microsoft.Azure.WebJobs.Description;
using NUnit.Framework;
using Shouldly;

namespace Carby.JobManager.Functions.Tests;

[TestFixture]
public class MessageProducerAttributeTests
{
    [Test]
    public void MessageProducerAttribute_HasBindingAttribute()
    {
        var attribute = typeof(MessageProducerAttribute);
        Assert.That(attribute, Has.Attribute<BindingAttribute>());
    }
    
    [Test]
    public void MessageProducerAttribute_HasAttributeUsageAttribute()
    {
        var attribute = typeof(MessageProducerAttribute);
        var attributeUsage = attribute.GetCustomAttributes(typeof(AttributeUsageAttribute), false);
        attributeUsage.ShouldNotBeEmpty();
        attributeUsage.ShouldContain(x => ((AttributeUsageAttribute)x).AllowMultiple == false);
        attributeUsage.ShouldContain(x => ((AttributeUsageAttribute)x).ValidOn == AttributeTargets.Parameter);
    }
}