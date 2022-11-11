using Carby.JobManager.Functions.CarbyClient;
using Carby.JobManager.Functions.Services;
using Carby.JobManager.Functions.Tasks.SplitterTask;
using Carby.JobManager.Functions.Tasks.TransformTask;
using Carby.JobManager.Functions.Tracing;
using Microsoft.ApplicationInsights.Extensibility;
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

        builder.AddExtension<TransformTaskExtensionConfigProvider>();
        builder.AddExtension<SplitterTaskExtensionConfigProvider>();
        builder.AddExtension<JobManagerClientExtensionConfigProvider>();
        builder.Services.AddSingleton<IMessagingService, StorageQueueMessagingService>();
        builder.Services.AddSingleton<INamedJobCollection, NamedJobCollection>();
        builder.Services.AddSingleton<IJobContextManagerService, JobContextManagerService>();
        builder.Services.AddSingleton<IJobHistoryService, JobHistoryService>();
        builder.Services.AddSingleton<ITelemetryModule, TelemetryModule>();

        return builder;
    }
}