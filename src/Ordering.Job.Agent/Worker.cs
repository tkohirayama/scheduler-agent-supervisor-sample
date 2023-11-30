using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;

namespace Ordering.Job.Agent;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IHostApplicationLifetime _appLifetime;
    private readonly QueueClient _requestQueueName;
    private readonly QueueClient _responseQueueName;
    private readonly IRemoteService _remoteService;
    public Worker(ILogger<Worker> logger, IHostApplicationLifetime appLifetime, IConfiguration configuration)
    {
        _logger = logger;
        _appLifetime = appLifetime;

        _appLifetime.ApplicationStarted.Register(OnStarted);
        _appLifetime.ApplicationStopped.Register(OnStopped);
        string connectionString = configuration.GetValue<string>("AZURE_STORAGE_CONNECTION_STRING") ?? throw new Exception();
        string requestQueueName = configuration.GetValue<string>("AZURE_STORAGE_REQUEST_QUEUE_NAME") ?? throw new Exception();
        string responseQueueName = configuration.GetValue<string>("AZURE_STORAGE_RESPONSE_QUEUE_NAME") ?? throw new Exception();
        _requestQueueName = new QueueClient(connectionString, requestQueueName);
        _responseQueueName = new QueueClient(connectionString, responseQueueName);
        _remoteService = new RemoteServiceMock(configuration);
    }

    void OnStarted()
    {
        _logger.LogInformation("Started Ordering.Job.Agent");
    }

    void OnStopped()
    {
        _logger.LogInformation("Stopped Ordering.Job.Agent");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if (await _requestQueueName.ExistsAsync(stoppingToken).ConfigureAwait(false))
            {
                try
                {
                    QueueProperties properties = await _requestQueueName.GetPropertiesAsync(stoppingToken).ConfigureAwait(false);
                    if (properties.ApproximateMessagesCount > 0)
                    {
                        QueueMessage[] retrievedMessage = await _requestQueueName.ReceiveMessagesAsync(1, cancellationToken: stoppingToken);
                        string orderingInfo = retrievedMessage[0].Body.ToString();
                        _logger.LogInformation("Received Ordering Info: {orderingInfo}", orderingInfo);

                        await _remoteService.PostOrderingInfoAsync(orderingInfo);
                        await _responseQueueName.SendMessageAsync($"RemoteService was Successful. {retrievedMessage[0].MessageId}"); 

                        // Delete Message
                        await _requestQueueName.DeleteMessageAsync(retrievedMessage[0].MessageId, retrievedMessage[0].PopReceipt, stoppingToken);
                    }
                    await Task.Delay(1000, stoppingToken);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Error Ordering.Job.Agent");
                }
            }

            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            await Task.Delay(1000, stoppingToken);
        }
    }
}
