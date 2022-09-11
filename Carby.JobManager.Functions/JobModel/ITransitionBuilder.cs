namespace Carby.JobManager.Functions.JobModel;

public interface ITransitionBuilder
{
    ITransitionBuilder From(string sourceTask);
    ITransitionBuilder To(string targetTask);
    ITransitionBuilder To(string targetTask, Func<IJobContext, bool> condition);
}