using Carby.JobManager.Functions.JobModel;

namespace Carby.JobManager.Functions;

internal interface INamedJobCollection : IDictionary<string, JobDescriptor>
{
}