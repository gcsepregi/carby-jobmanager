using Carby.JobManager.Functions.SimpleTask;
using Microsoft.Azure.WebJobs;

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
        return builder;
    }
}