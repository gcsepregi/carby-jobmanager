namespace Carby.JobManager.Functions.JobModel;

public interface ITransitionBuilder
{
    ITransitionBuilder From(string sourceTask);
    ITransitionBuilder To(string targetTask);
    ITransitionBuilder When(Func<IJobContext, bool> condition);
    ITransitionBuilder FanOut(Func<IJobContext, int> instanceCountProvider);
    ITransitionBuilder FanIn();
}