using System.Diagnostics;

namespace Carby.JobManager.Functions.Services;

public class CommonServices : ICommonServices
{
    public string GetJobName()
    {
        return Activity.Current!.GetBaggageItem("CurrentJobName") ?? "CarbyDefaultJob";
    }
}