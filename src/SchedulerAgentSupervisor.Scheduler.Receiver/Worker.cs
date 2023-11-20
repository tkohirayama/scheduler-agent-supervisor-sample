namespace SchedulerAgentSupervisor.Scheduler.Receiver;

using System.Threading.Tasks;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IHostApplicationLifetime _appLifetime;

    public Worker(ILogger<Worker> logger, IHostApplicationLifetime appLifetime)
    {
        _logger = logger;
        _appLifetime = appLifetime;

        _appLifetime.ApplicationStarted.Register(OnStarted);
        _appLifetime.ApplicationStopped.Register(OnStopped);
    }

    void OnStarted()
    {
        _logger.LogInformation("Active Receiver");
    }

    void OnStopped()
    {
        _logger.LogInformation("Close Receiver");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        string connectionString = Environment.GetEnvironmentVariable("AZURE_STORAGE_CONNECTION_STRING") ?? throw new Exception();
        string queueName = Environment.GetEnvironmentVariable("AZURE_STORAGE_QUEUE_NAME") ?? throw new Exception();
        var queueClient = new QueueClient(connectionString, queueName);

        if (await queueClient.ExistsAsync(stoppingToken).ConfigureAwait(false))
        {
            QueueProperties properties = await queueClient.GetPropertiesAsync(stoppingToken).ConfigureAwait(false);

            if (properties.ApproximateMessagesCount > 0)
            {
                QueueMessage[] retrievedMessage = await queueClient.ReceiveMessagesAsync(1, cancellationToken: stoppingToken);
                string theMessage = retrievedMessage[0].Body.ToString();
                _logger.LogInformation("Received Message: {}", theMessage);

                // TODO: Update DB Record

                // Delete Message
                await queueClient.DeleteMessageAsync(retrievedMessage[0].MessageId, retrievedMessage[0].PopReceipt, stoppingToken);

                await Task.Delay(10000, stoppingToken);
            }
        }
        _appLifetime.StopApplication();
    }
}
