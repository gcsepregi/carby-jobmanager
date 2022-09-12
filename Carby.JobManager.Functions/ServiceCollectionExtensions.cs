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
        services.AddSingleton<IJobDescriptorBuilder>(provider =>
        {
            var jobBuilder = new JobDescriptorBuilder();
            ((IJobDescriptorBuilder)jobBuilder).Name = name;
            jobDescriptorBuilderCb(jobBuilder);
            return jobBuilder;
        });
        
        return services;
    } 
}