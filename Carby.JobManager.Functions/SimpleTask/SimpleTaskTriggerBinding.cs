using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Azure.WebJobs.Host.Listeners;
using Microsoft.Azure.WebJobs.Host.Protocols;
using Microsoft.Azure.WebJobs.Host.Triggers;

namespace Carby.JobManager.Functions.SimpleTask;

public class SimpleTaskTriggerBinding : ITriggerBinding
{
    private readonly SimpleTaskTriggerBindingContext _context;
    private readonly IReadOnlyDictionary<string, Type> _bindingDataContract;

    public SimpleTaskTriggerBinding(SimpleTaskTriggerBindingContext context)
    {
        _context = context;
        _bindingDataContract = GetBindingDataContract();
    }

    public Task<ITriggerData> BindAsync(object value, ValueBindingContext context)
    {
        return Task.FromResult<ITriggerData>(new TriggerData(new SimpleTaskValueProvider(value), GetBindingData())
        {
            ReturnValueProvider = _context.SimpleTaskReturnValueHandler
        });
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
    public IReadOnlyDictionary<string, Type> BindingDataContract => _bindingDataContract;
    
    private IReadOnlyDictionary<string, object> GetBindingData()
    {
        return new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
    }
    
    private IReadOnlyDictionary<string, Type> GetBindingDataContract()
    {
        return new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase)
        {
            { "$return", typeof(object).MakeByRefType() }
        };
    }

}