using System.Text.Json;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using Ordering.Job.Domain.Messages;

namespace Ordering.Job.Agent;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IHostApplicationLifetime _appLifetime;
    private readonly QueueClient _requestQueue;
    private readonly QueueClient _responseQueue;
    private readonly IRemoteService _remoteService;
    public Worker(ILogger<Worker> logger, IHostApplicationLifetime appLifetime,
        IConfiguration configuration,
        IRemoteService remoteService)
    {
        _logger = logger;
        _appLifetime = appLifetime;

        _appLifetime.ApplicationStarted.Register(OnStarted);
        _appLifetime.ApplicationStopped.Register(OnStopped);
        string connectionString = configuration.GetValue<string>("AZURE_STORAGE_CONNECTION_STRING") ?? throw new Exception();
        string requestQueueName = configuration.GetValue<string>("AZURE_STORAGE_REQUEST_QUEUE_NAME") ?? throw new Exception();
        string responseQueueName = configuration.GetValue<string>("AZURE_STORAGE_RESPONSE_QUEUE_NAME") ?? throw new Exception();
        _requestQueue = new QueueClient(connectionString, requestQueueName);
        _responseQueue = new QueueClient(connectionString, responseQueueName);
        _remoteService = remoteService;
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
        if (await _requestQueue.ExistsAsync(stoppingToken).ConfigureAwait(false))
        {
            try
            {
                QueueProperties properties = await _requestQueue.GetPropertiesAsync(stoppingToken).ConfigureAwait(false);
                if (properties.ApproximateMessagesCount > 0)
                {
                    QueueMessage[] retrievedMessage = await _requestQueue.ReceiveMessagesAsync(10, cancellationToken: stoppingToken);
                    foreach (var message in retrievedMessage)
                    {
                        var request = JsonSerializer.Deserialize<RequestModel>(message.Body);
                        _logger.LogInformation("Received Ordering Info: {OrderId}", request!.OrderId);

                        try
                        {
                            await _remoteService.PostOrderingInfoAsync(request);
                            await _responseQueue.SendMessageAsync(JsonSerializer.Serialize(new ResponseModel { OrderId = request.OrderId }));
                        }
                        catch (RemoteServiceException rex)
                        {
                            _logger.LogError(rex, "Error Remote Service: {OrderId}", request.OrderId);
                        }

                        await _requestQueue.DeleteMessageAsync(message.MessageId, message.PopReceipt, stoppingToken);
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error Ordering.Job.Agent");
            }
            finally
            {
                _appLifetime.StopApplication();
            }
        }
    }
}
