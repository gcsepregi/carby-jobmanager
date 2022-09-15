namespace Carby.JobManager.Functions.Services;

internal class StorageManagerServiceBase
{
    protected static string GetStorageConnection()
        => Environment.GetEnvironmentVariable($"CarbyJobManager:StorageAccountConnection")!;
}