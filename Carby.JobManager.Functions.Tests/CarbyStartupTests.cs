using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using NSubstitute;
using NUnit.Framework;

namespace Carby.JobManager.Functions.Tests;

[TestFixture]
public class CarbyStartupTests
{
    [Test]
    public void CarbyStartup_CallsAddCarbyExtensionsOnBuilder()
    {
        IWebJobsStartup startup = new CarbyStartup();
        var builder = Substitute.For<IWebJobsBuilder>();
        
        startup.Configure(builder);
        
        builder.Received(1).AddCarbyExtensions();
    }
}