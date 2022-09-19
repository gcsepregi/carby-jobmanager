using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace Carby.JobManager.Functions;

public class ManagementApi
{
    [FunctionName(nameof(GetJobs))]
    public Task<IActionResult> GetJobs([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest request)
    {
        return Task.FromResult<IActionResult>(new OkObjectResult("HelloWorld"));
    }
}