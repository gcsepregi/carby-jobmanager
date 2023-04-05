using Carby.JobManager.Functions.JobModel;
using Carby.JobManager.Functions.Demo;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Startup))]
namespace Carby.JobManager.Functions.Demo;

public class Startup : FunctionsStartup
{
    public override void Configure(IFunctionsHostBuilder builder)
    {
        builder.Services.AddNamedJobDescriptor("SimpleTaskTest", BuildSimpleTaskTestJob);
    }

    private static void BuildSimpleTaskTestJob(IJobDescriptorBuilder builder)
    {
        builder
            .StartWith(nameof(SimpleTaskTester.SimpleTaskStartAsync))
            .EndWith(nameof(SimpleTaskTester.SimpleTaskEndAsync))
            .HandleFailureWith(nameof(SimpleTaskTester.SimpleTaskFailureAsync))
            .CleanUpWith(nameof(SimpleTaskTester.SimpleTaskCleanUpAsync))
            .AddTransition(transitionBuilder =>
                    transitionBuilder
                        .From(nameof(SimpleTaskTester.SimpleTaskStartAsync))
                        .To(nameof(SimpleTaskTester.SimpleTaskMiddleAsync))
            )
            .AddTransition(transitionBuilder =>
                    transitionBuilder
                        .From(nameof(SimpleTaskTester.SimpleTaskMiddleAsync))
                        .To(nameof(SimpleTaskTester.SimpleTaskEndAsync))
            )
            ;
    }
}