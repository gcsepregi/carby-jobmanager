using System.Text.Json;
using Carby.JobManager.Functions.Configurations;
using Carby.JobManager.Functions.Factories;
using Carby.JobManager.Functions.Interfaces;
using Carby.JobManager.Functions.Model;

namespace Carby.JobManager.Functions.MessageProducer;

internal class PointToPointMessageProducer : IMessageProducer
{
    private readonly string _target;
    private readonly IAzureStorageFactory _azureStorageFactory;


    public PointToPointMessageProducer(string target, IAzureStorageFactory azureStorageFactory)
    {
        _target = target;
        _azureStorageFactory = azureStorageFactory;
    }

    public async Task SendAsync(object message, MessageProducerOptions? options = default)
    {
        // Generate unique message id
        var messageId = Guid.NewGuid();

        // Store message to blob storage
        await AddMessageToBlobStorage(messageId, message);

        // Send control message to queue
        await SendControlMessageToTarget(messageId);
    }

    private async Task SendControlMessageToTarget(Guid messageId)
    {
        var queue = await _azureStorageFactory.CreateQueueClient(_target);
        await queue.SendMessageAsync(new BinaryData(JsonSerializer.Serialize(new ControlMessage
        {
            MessageId = messageId
        })));
    }

    private async Task AddMessageToBlobStorage(Guid messageId, object message)
    {
        var blobName = $"{messageId}.json";
        var containerName = $"msg-{_target}";
        var blobClient = await _azureStorageFactory.CreateBlobClient(containerName, blobName);
        await blobClient.UploadAsync(new BinaryData(JsonSerializer.Serialize(message)));
    }
}