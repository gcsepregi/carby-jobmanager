using System.Text.Json;
using Carby.JobManager.Functions.Attributes;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Azure.WebJobs.Host.Listeners;
using Microsoft.Azure.WebJobs.Host.Protocols;
using Microsoft.Azure.WebJobs.Host.Triggers;

namespace Carby.JobManager.Functions.AbstractCarbyTask;

internal abstract class AbstractCarbyTaskTriggerBinding<T> : ITriggerBinding
{
    private readonly AbstractCarbyTaskTriggerBindingContext<T> _context;

    protected AbstractCarbyTaskTriggerBinding(AbstractCarbyTaskTriggerBindingContext<T> context)
    {
        _context = context;
        BindingDataContract = GetBindingDataContract();
    }

    public async Task<ITriggerData> BindAsync(object value, ValueBindingContext context)
    {
        return new TriggerData(new AbstractCarbyTaskValueProvider(value), await GetBindingDataAsync())
        {
            ReturnValueProvider = _context.ReturnValueHandler
        };
    }

    public Task<IListener> CreateListenerAsync(ListenerFactoryContext context)
    {
        return Task.FromResult(CreateListener(context, _context));
    }
    
    public ParameterDescriptor ToParameterDescriptor()
    {
        return new ParameterDescriptor();
    }

    public Type TriggerValueType => typeof(object);
    public IReadOnlyDictionary<string, Type> BindingDataContract { get; }

    protected abstract IListener CreateListener(ListenerFactoryContext context, AbstractCarbyTaskTriggerBindingContext<T> triggerBindingContext);

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