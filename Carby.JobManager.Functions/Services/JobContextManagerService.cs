using System.Diagnostics;
using System.Text.Json;
using Azure.Data.Tables;
using Carby.JobManager.Functions.JobModel;

namespace Carby.JobManager.Functions.Services;

internal sealed class JobContextManagerService : StorageManagerServiceBase, IJobContextManagerService
{
    private const string JobContextsTableName = "jobcontexts";

    public async Task<IJobContext> ReadJobContextAsync()
    {
        var jobName = Activity.Current?.GetBaggageItem(ICommonServices.CurrentJobNameKey) ?? throw new InvalidOperationException("Activity must already be started and baggage and tags filled");
        var jobId = Activity.Current?.GetBaggageItem(ICommonServices.InternalJobIdKey) ?? throw new InvalidOperationException("Activity must already be started and baggage and tags filled");

        var tableClient = new TableClient(GetStorageConnection(), JobContextsTableName);
        var tableEntity = await tableClient.GetEntityAsync<TableEntity>(jobName, jobId);

        var jobContext = new JobContext(jobName!);
        foreach (var (key, value) in tableEntity.Value)
        {
            jobContext[key] = value;
        }
        return jobContext;
    }

    public async Task PersistJobContextAsync(IJobContext jobContext)
    {
        var jobName = Activity.Current?.GetBaggageItem(ICommonServices.CurrentJobNameKey) ?? throw new InvalidOperationException("Activity must already be started and baggage and tags filled");
        var tableClient = new TableClient(GetStorageConnection(), JobContextsTableName);
        await tableClient.CreateIfNotExistsAsync();
        var tableEntity = new TableEntity(jobName, jobContext.JobId);
        foreach (var (key, value) in jobContext)
        {
            tableEntity[key] = value;
        }
        await tableClient.UpsertEntityAsync(tableEntity);
    }

    public Task<IJobContext> ConvertUserType<T>(string jobName, T userObject)
    {
        var dictObject = JsonSerializer.Deserialize<Dictionary<string, object>>(JsonSerializer.Serialize(userObject));
        return Task.FromResult<IJobContext>(new JobContext(jobName, dictObject));
    }

}