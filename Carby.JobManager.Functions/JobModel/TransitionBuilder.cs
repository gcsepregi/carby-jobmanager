namespace Carby.JobManager.Functions.JobModel;

internal sealed  class TransitionBuilder : ITransitionBuilder
{
    private readonly TransitionDescriptor _descriptor = new ();
    
    public ITransitionBuilder From(string sourceTask)
    {
        _descriptor.FromTask = sourceTask;
        return this;
    }

    public ITransitionBuilder To(string targetTask)
    {
        _descriptor.ToTask = targetTask;
        return this;
    }
    
    public ITransitionBuilder When(Func<IJobContext, bool> condition)
    {
        _descriptor.When = condition;
        return this;
    }

    public ITransitionBuilder Parallelization(Func<IJobContext, int> instanceCountProvider)
    {
        _descriptor.FanOut = instanceCountProvider;
        return this;
    }

    public ITransitionBuilder Synchronize()
    {
        _descriptor.FanIn = true;
        return this;
    }

    public TransitionDescriptor Build()
    {
        return _descriptor;
    }

}