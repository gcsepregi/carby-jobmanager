using Microsoft.Azure.WebJobs.Host.Bindings;

namespace Carby.JobManager.Functions.SimpleTask;

internal class SimpleTaskReturnValueHandler : IValueBinder
{
    private readonly SimpleTaskExtensionConfigProvider _configProvider;

    public SimpleTaskReturnValueHandler(SimpleTaskExtensionConfigProvider configProvider)
    {
        _configProvider = configProvider;
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
    
    public Task SetValueAsync(object value, CancellationToken cancellationToken)
    {
        Console.WriteLine("ReturnHandler: SetValueAsync");
        return Task.CompletedTask;
    }
}