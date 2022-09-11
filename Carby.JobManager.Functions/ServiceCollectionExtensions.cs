using Carby.JobManager.Functions.JobModel;
using Microsoft.Extensions.DependencyInjection;

namespace Carby.JobManager.Functions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddNamedJobDescriptor(
        this IServiceCollection services, 
        string name,
        Action<IJobDescriptorBuilder> jobDescriptorBuilderCb)
    {
        services.AddSingleton<INamedJobCollection, NamedJobCollection>();

        var collection = services.BuildServiceProvider().GetService<INamedJobCollection>();
        if (collection == null)
        {
            throw new InvalidOperationException(
                $"No implementation of {nameof(INamedJobCollection)} had been registered");
        }
        
        var builder = new JobDescriptorBuilder();
        jobDescriptorBuilderCb(builder);
        collection[name] = builder.Build();
        return services;
    } 
}