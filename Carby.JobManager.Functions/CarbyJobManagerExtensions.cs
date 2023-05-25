using Microsoft.Azure.WebJobs;

namespace Carby.JobManager.Functions;

public static class CarbyJobManagerExtensions
{
    
    public static IWebJobsBuilder AddCarbyExtensions(this IWebJobsBuilder builder)
    {
        if (builder == null)
        {
            throw new ArgumentNullException(nameof(builder));
        }
        
        builder.AddExtension<MessageProducerExtensionConfigProvider>();

        return builder;
    }
    
}