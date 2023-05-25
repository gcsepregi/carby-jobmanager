using Microsoft.Azure.WebJobs;
using NSubstitute;
using NUnit.Framework;

namespace Carby.JobManager.Functions.Tests;

[TestFixture]
public class CarbyJobManagerExtensionsTests
{
    [Test]
    public void AddCarbyExtensions_ReturnsBuilder()
    {
        var builder = Substitute.For<IWebJobsBuilder>();
        
        var result = builder.AddCarbyExtensions();
        
        Assert.That(result, Is.EqualTo(builder));
    }
    
    [Test]
    public void AddCarbyExtensions_ThrowsOnNullBuilder()
    {
        IWebJobsBuilder? builder = null;
        
        Assert.That(() => builder!.AddCarbyExtensions(), Throws.ArgumentNullException);
    }
    
    [Test]
    public void AddCarbyExtensions_AddsMessageProducerExtensionConfigProvider()
    {
        var builder = Substitute.For<IWebJobsBuilder>();
        
        builder.AddCarbyExtensions();
        
        builder.Received(1).AddExtension<MessageProducerExtensionConfigProvider>();
    }
}