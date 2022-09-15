namespace Carby.JobManager.Functions.Services;

internal class StorageManagerServiceBase
{
    protected static string GetStorageConnection()
        => Environment.GetEnvironmentVariable($"CarbyJobManager:StorageAccountConnection")!;

    protected static TimeSpan GetMessageVisibilityTimeout(string? queueName)
        => TimeSpan.Parse(
            Environment.GetEnvironmentVariable($"CarbyJobManager:{queueName}:MessageVisibilityTimeout")
            ?? Environment.GetEnvironmentVariable("CarbyJobManager:MessageVisibilityTimeout")
            ?? "00:00:30"
        );

    protected static int GetParallelMessageCount(string? queueName)
        => int.Parse(
            Environment.GetEnvironmentVariable($"CarbyJobManager:{queueName}:ParallelMessageCount")
            ?? Environment.GetEnvironmentVariable("CarbyJobManager:ParallelMessageCount")
            ?? "10"
        );

    protected static int GetMessageRetryCount(string? queueName)
        => int.Parse(
            Environment.GetEnvironmentVariable($"CarbyJobManager:{queueName}:MessageRetryCount")
            ?? Environment.GetEnvironmentVariable("CarbyJobManager:MessageRetryCount")
            ?? "10"
        );
}