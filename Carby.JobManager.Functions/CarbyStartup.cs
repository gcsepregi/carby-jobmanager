using Carby.JobManager.Functions;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;

[assembly: WebJobsStartup(typeof(CarbyStartup))]
namespace Carby.JobManager.Functions;

public class CarbyStartup : IWebJobsStartup
{
    public void Configure(IWebJobsBuilder builder)
    {
        builder.AddCarbyExtensions();
    }
}