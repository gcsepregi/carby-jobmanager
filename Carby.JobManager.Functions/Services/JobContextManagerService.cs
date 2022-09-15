using System.Text.Json;
using Azure.Data.Tables;
using Carby.JobManager.Functions.JobModel;

namespace Carby.JobManager.Functions.Services;

internal sealed class JobContextManagerService : StorageManagerServiceBase, IJobContextManagerService
{
    public Task<IJobContext> ReadJobContextAsync(string? jobName)
    {
        throw new NotImplementedException();
    }

    public async Task PersistJobContextAsync(string? jobName, IJobContext jobContext)
    {
        var tableClient = new TableClient(GetStorageConnection(), "jobcontexts");
        await tableClient.CreateIfNotExistsAsync();
        var tableEntity = new TableEntity(jobName, jobContext.JobId);
        foreach (var (key, value) in jobContext)
        {
            tableEntity[key] = value;
        }
        await tableClient.AddEntityAsync(tableEntity);
    }

    public Task<IJobContext> ConvertUserType<T>(string jobName, T userObject)
    {
        var dictObject = JsonSerializer.Deserialize<Dictionary<string, object>>(JsonSerializer.Serialize(userObject));
        return Task.FromResult<IJobContext>(new JobContext(jobName, dictObject));
    }

}