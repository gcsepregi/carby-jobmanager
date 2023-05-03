using Carby.JobManager.Functions.Factories;
using Carby.JobManager.Functions.MessageProducer;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.DependencyInjection;

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
        
        builder.Services.AddSingleton<IAzureStorageFactory, AzureStorageFactory>();
        return builder;
    }
}