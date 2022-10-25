using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Carby.JobManager.Functions.Tracing;

public static class CarbyTraceHelper
{
    public const string CarbyEventSourceName = "Carby.Events.Common";

    private static readonly ActivitySource ActivitySource = new("CarbyJobManager");
    private static readonly DiagnosticSource CarbyEventDiagnosticSource = new DiagnosticListener(CarbyEventSourceName);

    public static bool IsEnabled(this string name)
    {
        return CarbyEventDiagnosticSource.IsEnabled(name);
    }

    public static Activity CreateActivity(this string operationName, ActivityKind activityKind = ActivityKind.Producer)
    {
        return ActivitySource.CreateActivity(operationName, activityKind) ?? new Activity(operationName);
    }

    public static void OnActivityImport(this Activity activity, object? context = null)
    {
        CarbyEventDiagnosticSource.OnActivityImport(activity, context);
    }
    
    public static void StartActivity(this Activity activity, object? context = null)
    {
        activity.Start();
        CarbyEventDiagnosticSource.Write($"{activity.OperationName}.Start", context);
    }
    
    public static void StopActivity(this Activity activity, object? context = null)
    {
        CarbyEventDiagnosticSource.Write($"{activity.OperationName}.Stop", context);
        activity.Stop();
    }
    
    public static void FillActivityFromDistributedContext(this Activity activity, IDictionary<string, string> headers)
    {
        DistributedContextPropagator.Current.ExtractTraceIdAndState(headers,
            (object? carrier, string name, out string? value, out IEnumerable<string>? values) =>
            {
                if (carrier == null)
                {
                    value = null;
                    values = null;
                    return;
                }
                
                values = default;
                var dictionary = (IDictionary<string, string>)carrier;
                value = dictionary.ContainsKey(name) ? dictionary[name] : default;
            }, out var traceId, out var traceState);

        if (string.IsNullOrEmpty(traceId))
        {
            return;
        }

        activity.SetParentId(traceId);
        if (!string.IsNullOrEmpty(traceState))
        {
            activity.TraceStateString = traceState;
        }

        var baggage = DistributedContextPropagator.Current.ExtractBaggage(headers,
            static (object? carrier, string fieldName, out string? fieldValue, out IEnumerable<string>? fieldValues) =>
            {
                fieldValues = default;
                var carrierDict = (IDictionary<string, string>)carrier!;
                fieldValue = carrierDict.ContainsKey(fieldName) ? carrierDict[fieldName] : default;
            });

        if (baggage is null)
        {
            return;
        }
            
        foreach (var baggageItem in baggage)
        {
            activity.AddBaggage(baggageItem.Key, baggageItem.Value);
        }
    }

}