using Carby.JobManager.Functions.CarbyClient;
using Carby.JobManager.Functions.Services;
using Carby.JobManager.Functions.SimpleTask;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.DependencyInjection;

namespace Carby.JobManager.Functions;

public static class CarbyWebJobsBuilderExtensions
{
    public static IWebJobsBuilder AddCarbyExtensions(this IWebJobsBuilder builder)
    {
        if (builder == null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        builder.AddExtension<SimpleTaskExtensionConfigProvider>();
        builder.AddExtension<JobManagerClientExtionConfigProvider>();
        builder.Services.AddSingleton<ICommonServices, CommonServices>();
        builder.Services.AddSingleton<IMessagingService, ServiceBusMessagingService>();
        return builder;
    }
}