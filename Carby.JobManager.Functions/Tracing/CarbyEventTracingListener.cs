using System.Diagnostics;
using Carby.JobManager.Functions.Services;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;

namespace Carby.JobManager.Functions.Tracing;

internal sealed class CarbyEventTracingListener : IObserver<KeyValuePair<string, object?>>, IDisposable
{
    private readonly TelemetryClient _client;
    private readonly IJobHistoryService _jobHistoryService;

    public CarbyEventTracingListener(TelemetryClient client, IJobHistoryService jobHistoryService)
    {
        _client = client;
        _jobHistoryService = jobHistoryService;
    }

    public void OnCompleted()
    {
        // not required to implement
    }

    public void OnError(Exception error)
    {
        // not required to implement
    }

    public void OnNext(KeyValuePair<string, object?> value)
    {
        var key = value.Key;
        var activity = Activity.Current;

        if (activity == null)
        {
            return;
        }

        if (key.EndsWith("Start"))
        {
            _jobHistoryService.SaveHistoryItem(activity);
        }
        else if (key.EndsWith("Stop"))
        {
            var instanceId = activity.Tags
                .FirstOrDefault(kv => ICommonServices.TaskInstanceId.Equals(kv.Key)).Value;
            var requestData = _jobHistoryService.LoadHistoryItem(instanceId);
            var rq = ToRequest(requestData, activity);
            _client.TrackRequest(rq);
        }
    }

    public void Dispose()
    {
        // not required to implement
    }

    private static RequestTelemetry ToRequest(IDictionary<string, object> requestData, Activity activity)
    {
        Debug.Assert(activity.Id != null, "Activity must be started prior calling this method");

        var telemetry = new RequestTelemetry { Name = activity.OperationName };

        var operationContext = telemetry.Context.Operation;
        operationContext.Name = activity.OperationName;

        operationContext.Id = requestData["traceId"] as string;
        telemetry.Id = requestData["spanId"] as string;
        telemetry.Timestamp = requestData["startTimeUtc"] as DateTimeOffset? ?? default;
        telemetry.Duration = DateTimeOffset.UtcNow - telemetry.Timestamp;

        if (string.IsNullOrEmpty(operationContext.ParentId) && activity.ParentSpanId != default)
        {
            operationContext.ParentId = requestData["parentSpanId"] as string;
        }

        foreach (var item in activity.Baggage)
        {
            if (!telemetry.Properties.ContainsKey(item.Key))
            {
                telemetry.Properties.Add(item);
            }
        }

        foreach (var item in activity.Tags)
        {
            if (!telemetry.Properties.ContainsKey(item.Key))
            {
                telemetry.Properties.Add(item);
            }
        }

        return telemetry;
    }
}