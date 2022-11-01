using System.Text.Json;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Azure.WebJobs.Host.Listeners;
using Microsoft.Azure.WebJobs.Host.Protocols;
using Microsoft.Azure.WebJobs.Host.Triggers;

namespace Carby.JobManager.Functions.SimpleTask;

internal sealed class SimpleTaskTriggerBinding : ITriggerBinding
{
    private readonly SimpleTaskTriggerBindingContext _context;

    public SimpleTaskTriggerBinding(SimpleTaskTriggerBindingContext context)
    {
        _context = context;
        BindingDataContract = GetBindingDataContract();
    }

    public async Task<ITriggerData> BindAsync(object value, ValueBindingContext context)
    {
        return new TriggerData(new SimpleTaskValueProvider(value), await GetBindingDataAsync())
        {
            ReturnValueProvider = _context.SimpleTaskReturnValueHandler
        };
    }

    public Task<IListener> CreateListenerAsync(ListenerFactoryContext context)
    {
        return Task.FromResult<IListener>(new SimpleTaskListener(context, _context));
    }

    public ParameterDescriptor ToParameterDescriptor()
    {
        return new TriggerParameterDescriptor
        {
            Name = "SimpleTask",
            Type = "MySimpleTaskTriggerCustomType"
        };
    }

    public Type TriggerValueType => typeof(object);
    public IReadOnlyDictionary<string, Type> BindingDataContract { get; }

    private async Task<IReadOnlyDictionary<string, object>> GetBindingDataAsync()
    {
        var data = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        var method = _context.Method!;
        var jobContext = await _context.JobContextManager!.ReadJobContextAsync();

        foreach (var otherParam in method.GetParameters())
        {
            if (jobContext.ContainsKey(otherParam.Name!))
            {
                data[otherParam.Name!] = JsonSerializer.Deserialize(JsonSerializer.Serialize(jobContext[otherParam.Name!]), otherParam.ParameterType)!;
            }
        }
        
        return data;
    }
    
    private IReadOnlyDictionary<string, Type> GetBindingDataContract()
    {
        var contract = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase)
        {
            { "$return", typeof(object).MakeByRefType() }
        };

        var parameter = _context.Parameter!;
        var method = _context.Method!;
        
        contract[parameter.Name!] = parameter.ParameterType;

        foreach (var otherParam in method.GetParameters())
        {
            contract[otherParam.Name!] = otherParam.ParameterType;
        }

        return contract;
    }

}