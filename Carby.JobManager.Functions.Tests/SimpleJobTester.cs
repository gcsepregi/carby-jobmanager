using System.Threading.Tasks;
using Carby.JobManager.Functions.Attributes;
using Carby.JobManager.Functions.CarbyClient;
using Carby.JobManager.Functions.Tests.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace Carby.JobManager.Functions.Tests;

public class SimpleJobTester
{
    [FunctionName(nameof(StartJobTest))]
    public async Task<IActionResult> StartJobTest(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] CustomMessageTrigger req,
        [JobManagerClient] IJobManagerClient client)
    {
        var activity = client.CreateActivity("JobTest");
        activity.AddBaggage("Greeting", req.Greeting);
        activity.AddBaggage("Name", req.Name);
        client.StartActivity(activity);
        await client.StartJobAsync("SimpleTaskTest", req);
        client.StopActivity(activity);
        return new OkResult();
    }

}