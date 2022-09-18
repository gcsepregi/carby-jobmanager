using System.Diagnostics;
using System.Text.Json;
using Azure.Data.Tables;

namespace Carby.JobManager.Functions.Services;

internal sealed class JobHistoryService : StorageManagerServiceBase, IJobHistoryService
{
    public void SaveHistoryItem(Activity activity)
    {
        var jobName = activity.GetBaggageItem(ICommonServices.CurrentJobNameKey);
        var jobId = activity.GetBaggageItem(ICommonServices.InternalJobIdKey);
        var taskInstanceId = activity.Tags
                                 .FirstOrDefault(kv =>
                                     ICommonServices.TaskInstanceIdKey.Equals(kv.Key))
                                 .Value ??
                             Guid.NewGuid().ToString();
        var tableClient = new TableClient(GetStorageConnection(), "jobhistory");
        tableClient.CreateIfNotExists();
        var entry = new TableEntity(jobName, taskInstanceId)
        {
            ["jobId"] = jobId,
            ["operationName"] = activity.OperationName,
            ["traceId"] = activity.TraceId.ToHexString(),
            ["spanId"] = activity.SpanId.ToHexString(),
            ["parentSpanId"] = activity.ParentSpanId.ToHexString(),
            ["baggage"] = JsonSerializer.Serialize(activity.Baggage),
            ["tags"] = JsonSerializer.Serialize(activity.Tags),
            ["startTimeUtc"] = activity.StartTimeUtc
        };

        tableClient.AddEntity(entry);
    }

    public IDictionary<string, object> LoadHistoryItem(string? instanceId)
    {
        var jobName = Activity.Current!.GetBaggageItem(ICommonServices.CurrentJobNameKey);
        var tableClient = new TableClient(GetStorageConnection(), "jobhistory");
        return tableClient.GetEntity<TableEntity>(jobName, instanceId).Value;
    }
}