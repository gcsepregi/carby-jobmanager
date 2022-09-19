namespace Carby.JobManager.Functions.JobModel;

public interface IJobDescriptorBuilder
{
    internal string Name { get; set; }
    IJobDescriptorBuilder StartWith(string taskName);
    IJobDescriptorBuilder EndWith(string taskName);
    IJobDescriptorBuilder HandleFailureWith(string taskName);
    IJobDescriptorBuilder CleanUpWith(string taskName);
    IJobDescriptorBuilder AddTransition(Action<ITransitionBuilder> builder);
    internal JobDescriptor Build();
}