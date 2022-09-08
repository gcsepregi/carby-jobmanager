using Microsoft.Azure.WebJobs.Host.Bindings;

namespace Carby.JobManager.Functions.SimpleTask;

public class SimpleTaskValueProvider : IValueBinder
{
    public SimpleTaskValueProvider(object value)
    {
        throw new NotImplementedException();
    }

    public Task<object> GetValueAsync()
    {
        throw new NotImplementedException();
    }

    public string ToInvokeString()
    {
        throw new NotImplementedException();
    }

    public Type Type { get; }
    public Task SetValueAsync(object value, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}