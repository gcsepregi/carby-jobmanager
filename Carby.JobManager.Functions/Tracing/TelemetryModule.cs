using System.Diagnostics;
using Carby.JobManager.Functions.Services;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;

namespace Carby.JobManager.Functions.Tracing;

internal sealed class TelemetryModule : ITelemetryModule, IDisposable, IObserver<DiagnosticListener>
{
    private readonly IJobHistoryService _jobHistoryService;
    private TelemetryClient? _client;
    private readonly List<IDisposable> _subscriptions = new();
    private readonly List<IDisposable> _listeners = new();

    private IDisposable? _allSubscriptions;

    public TelemetryModule(IJobHistoryService jobHistoryService)
    {
        _jobHistoryService = jobHistoryService;
    }

    public void Initialize(TelemetryConfiguration configuration)
    {
        _client = new TelemetryClient(configuration);
        _allSubscriptions = DiagnosticListener.AllListeners.Subscribe(this);
    }

    public void Dispose()
    {
        foreach (var subscription in _subscriptions)
        {
            subscription.Dispose();
        }

        foreach (var listener in _listeners)
        {
            listener.Dispose();
        }
            
        _allSubscriptions?.Dispose();
    }

    public void OnCompleted()
    {
        // Not implemented
    }

    public void OnError(Exception error)
    {
        // Not implemented
    }

    public void OnNext(DiagnosticListener value)
    {
        if (value.Name != CarbyTraceHelper.CarbyEventSourceName)
        {
            return;
        }
            
        var listener = new CarbyEventTracingListener(_client!, _jobHistoryService);
        var subscription = value.Subscribe(listener);
                
        _subscriptions.Add(subscription);
        _listeners.Add(listener);
    }
}