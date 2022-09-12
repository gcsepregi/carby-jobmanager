namespace Carby.JobManager.Functions.JobModel;

internal sealed class JobDescriptorBuilder : IJobDescriptorBuilder
{
    private readonly JobDescriptor _jobDescriptor;
    string IJobDescriptorBuilder.Name { get; set; }

    public JobDescriptorBuilder()
    {
        _jobDescriptor = new JobDescriptor();
    }

    public IJobDescriptorBuilder StartWith(string taskName)
    {
        _jobDescriptor.StartTask = taskName;
        return this;
    }
    
    public IJobDescriptorBuilder EndWith(string taskName)
    {
        _jobDescriptor.EndTask = taskName;
        return this;
    }
    
    public IJobDescriptorBuilder HandleFailureWith(string taskName)
    {
        _jobDescriptor.HandleFailureTask = taskName;
        return this;
    }
    
    public IJobDescriptorBuilder CleanUpWith(string taskName)
    {
        _jobDescriptor.CleanUpTask = taskName;
        return this;
    }

    JobDescriptor IJobDescriptorBuilder.Build()
    {
        return _jobDescriptor;
    }

    public IJobDescriptorBuilder AddTransitions(Action<ITransitionBuilder> builderCb)
    {
        var builder = new TransitionBuilder();
        builderCb(builder);
        _jobDescriptor.Transitions = builder.Build();
        return this;
    }
}