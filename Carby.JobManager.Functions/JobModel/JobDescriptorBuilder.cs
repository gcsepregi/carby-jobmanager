namespace Carby.JobManager.Functions.JobModel;

internal sealed class JobDescriptorBuilder : IJobDescriptorBuilder
{
    private readonly JobDescriptor _jobDescriptor = new()
    {
        Transitions = new Dictionary<string, ICollection<TransitionDescriptor>>()
    };
    
    string IJobDescriptorBuilder.Name { get; set; } = string.Empty;

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

    public IJobDescriptorBuilder AddTransition(Action<ITransitionBuilder> builderCb)
    {
        var builder = new TransitionBuilder();
        builderCb(builder);
        var transition = builder.Build();
        if (!_jobDescriptor.Transitions.ContainsKey(transition.FromTask))
        {
            _jobDescriptor.Transitions[transition.FromTask] = new List<TransitionDescriptor>();
        }
        _jobDescriptor.Transitions[transition.FromTask].Add(transition);

        return this;
    }
}