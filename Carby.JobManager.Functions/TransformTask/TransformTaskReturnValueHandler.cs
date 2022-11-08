using System.Text.Json;
using Carby.JobManager.Functions.Services;
using Microsoft.Azure.WebJobs.Host.Bindings;

namespace Carby.JobManager.Functions.TransformTask;

internal class TransformTaskReturnValueHandler : IValueBinder
{
    private readonly IJobContextManagerService _jobContextManager;

    public TransformTaskReturnValueHandler(IJobContextManagerService jobContextManager)
    {
        _jobContextManager = jobContextManager;
    }

    public Task<object> GetValueAsync()
    {
        return null!;
    }

    public string ToInvokeString()
    {
        return string.Empty;
    }

    public Type Type => typeof(object);
    
    public async Task SetValueAsync(object value, CancellationToken cancellationToken)
    {
        var dictionaryObject = JsonSerializer.Deserialize<Dictionary<string, object>>(JsonSerializer.Serialize(value));
        var jobContext = await _jobContextManager.ReadJobContextAsync();
        foreach (var (key, val) in dictionaryObject!)
        {
            jobContext[key] = val;
        }

        await _jobContextManager.PersistJobContextAsync(jobContext);
    }
}