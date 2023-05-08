using Carby.JobManager.Functions.Attributes;
using Carby.JobManager.Functions.Factories;
using Carby.JobManager.Functions.Interfaces;
using Microsoft.Azure.WebJobs.Host.Config;

namespace Carby.JobManager.Functions.MessageProducer;

// ReSharper disable once ClassNeverInstantiated.Global
// This class is instantiated by the Azure Functions runtime.
internal sealed class MessageProducerExtensionConfigProvider : IExtensionConfigProvider
{
    private readonly IAzureStorageFactory _azureStorageFactory;

    public MessageProducerExtensionConfigProvider(IAzureStorageFactory azureStorageFactory)
    {
        _azureStorageFactory = azureStorageFactory;
    }
    
    public void Initialize(ExtensionConfigContext context)
    {
        var bindingRule = context.AddBindingRule<MessageProducerAttribute>();
        bindingRule.BindToInput(CreateMessageProducerFromAttribute);
    }

    private IMessageProducer CreateMessageProducerFromAttribute(MessageProducerAttribute producerAttribute)
    {
        return new PointToPointMessageProducer(producerAttribute.Target, _azureStorageFactory);
    }
    
}