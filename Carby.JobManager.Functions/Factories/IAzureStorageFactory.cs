using Azure.Storage.Blobs;
using Azure.Storage.Queues;

namespace Carby.JobManager.Functions.Factories;

internal interface IAzureStorageFactory
{
    Task<BlobClient> CreateBlobClient(string containerName, string blobName);
    
    Task<QueueClient> CreateQueueClient(string queueName);
}