namespace Carby.JobManager.Functions.JobModel;

public interface IJobDescriptorBuilder
{
    string Name { get; set; }
    IJobDescriptorBuilder StartWith(string taskName);
    IJobDescriptorBuilder EndWith(string taskName);
    IJobDescriptorBuilder HandleFailureWith(string taskName);
    IJobDescriptorBuilder CleanUpWith(string taskName);
    IJobDescriptorBuilder AddTransitions(Action<ITransitionBuilder> builder);
    internal JobDescriptor Build();
}