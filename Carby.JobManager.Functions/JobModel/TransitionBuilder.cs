namespace Carby.JobManager.Functions.JobModel;

internal sealed  class TransitionBuilder : ITransitionBuilder
{
    private string? _currentSourceTask;
    private readonly IDictionary<string, ICollection<TransitionDescriptor>> _transitions = new Dictionary<string, ICollection<TransitionDescriptor>>();

    public ITransitionBuilder From(string sourceTask)
    {
        _currentSourceTask = sourceTask;
        return this;
    }

    public ITransitionBuilder To(string targetTask)
    {
        return To(targetTask, context => true);
    }
    
    public ITransitionBuilder To(string targetTask, Func<IJobContext, bool> condition)
    {
        if (_currentSourceTask == null)
        {
            throw new InvalidOperationException("Must call 'From' to create a source first!");
        }

        if (!_transitions.ContainsKey(_currentSourceTask))
        {
            _transitions[_currentSourceTask] = new List<TransitionDescriptor>();
        }
        
        _transitions[_currentSourceTask].Add(new TransitionDescriptor
        {
            FromTask = _currentSourceTask,
            ToTask = targetTask,
            When = condition
        });

        return this;
    }

    public IDictionary<string, ICollection<TransitionDescriptor>> Build()
    {
        return _transitions;
    }

}