using Azure.Storage.Blobs;
using Azure.Storage.Queues;

namespace Carby.JobManager.Functions.Factories;

internal sealed class AzureStorageFactory : IAzureStorageFactory
{
    private static string GetStorageConnection()
        => Environment.GetEnvironmentVariable("CarbyJobManager:StorageAccountConnection")!;
    
    public async Task<BlobClient> CreateBlobClient(string containerName, string blobName)
    {
        var containerClient = new BlobContainerClient(GetStorageConnection(), containerName);
        await containerClient.CreateIfNotExistsAsync();
        var blobClient = containerClient.GetBlobClient(blobName);
        return blobClient;
    }

    public async Task<QueueClient> CreateQueueClient(string queueName)
    {
        var queueClient = new QueueClient(GetStorageConnection(), queueName.ToLowerInvariant());
        await queueClient.CreateIfNotExistsAsync();
        return queueClient;
    }
}