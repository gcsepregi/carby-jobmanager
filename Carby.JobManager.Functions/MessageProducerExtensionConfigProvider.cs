using Microsoft.Azure.WebJobs.Host.Config;

namespace Carby.JobManager.Functions;

public class MessageProducerExtensionConfigProvider : IExtensionConfigProvider
{
    public void Initialize(ExtensionConfigContext context)
    {
        var binding = context.AddBindingRule<MessageProducerAttribute>();
        binding.BindToInput(CreateMessageProviderFromAttribute);
    }

    private IMessageProducer CreateMessageProviderFromAttribute(MessageProducerAttribute arg)
    {
        return new PointToPointMessageProducer(arg.MessageTarget);
    }
}