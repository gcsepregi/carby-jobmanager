using Microsoft.Azure.WebJobs.Host.Bindings;

namespace Carby.JobManager.Functions.SimpleTask;

public class SimpleTaskValueProvider : IValueBinder
{
    private object _value;

    public SimpleTaskValueProvider(object value)
    {
        _value = value;
    }

    public Task<object> GetValueAsync()
    {
        return Task.FromResult(_value);
    }

    public string ToInvokeString()
    {
        return _value.ToString() ?? string.Empty;
    }

    public Type Type => typeof(object);
    
    public Task SetValueAsync(object value, CancellationToken cancellationToken)
    {
        _value = value;
        return Task.CompletedTask;
    }
}