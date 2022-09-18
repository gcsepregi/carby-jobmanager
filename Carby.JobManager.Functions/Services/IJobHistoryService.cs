using System.Diagnostics;

namespace Carby.JobManager.Functions.Services;

internal interface IJobHistoryService
{
    void SaveHistoryItem(Activity activity);
    IDictionary<string, object> LoadHistoryItem(string? instanceId);
}