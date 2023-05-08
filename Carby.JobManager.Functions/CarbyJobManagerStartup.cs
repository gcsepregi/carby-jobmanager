using Carby.JobManager.Functions;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;

[assembly: WebJobsStartup(typeof(CarbyJobManagerStartup.CarbyStartup))]
namespace Carby.JobManager.Functions;

// ReSharper disable once ClassNeverInstantiated.Global
// This class is instantiated by the Azure Functions runtime.
public class CarbyJobManagerStartup
{

    public class CarbyStartup : IWebJobsStartup
    {
        public void Configure(IWebJobsBuilder builder)
        {
            builder.AddCarbyExtensions();
        }
    }
}