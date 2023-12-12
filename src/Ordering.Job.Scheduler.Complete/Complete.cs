using System.Text.Json;
using Azure.Storage.Queues.Models;

namespace Ordering.Job.Scheduler.Complete;
public class Complete : IComplete
{
    private readonly ILogger<Complete> _logger;
    private readonly OrderingJobContext _dbContext;
    private readonly QueueClient _responseQueue;
    private readonly IOrderingJobStateRepository _orderingJobStateRepository;
    public Complete(ILogger<Complete> logger,
        OrderingJobContext dbContext,
        QueueClient requestQueue,
        IOrderingJobStateRepository orderingJobStateRepository)
    {
        _logger = logger;
        _dbContext = dbContext;
        _responseQueue = requestQueue;
        _orderingJobStateRepository = orderingJobStateRepository;
    }

    public async Task Execute(CancellationToken cancellationToken)
    {
        try
        {
            if (await _responseQueue.ExistsAsync(cancellationToken).ConfigureAwait(false))
            {
                QueueProperties properties = await _responseQueue.GetPropertiesAsync(cancellationToken).ConfigureAwait(false);
                if (properties.ApproximateMessagesCount > 0)
                {
                    QueueMessage[] retrievedMessage = await _responseQueue.ReceiveMessagesAsync(10, cancellationToken: cancellationToken);
                    var completeModel = JsonSerializer.Deserialize<ResponseModel>(retrievedMessage[0].Body);

                    await using var transaction = await _dbContext.BeginTransactionAsync();
                    var state = await _orderingJobStateRepository.FindAsync(completeModel!.OrderId).ConfigureAwait(false);

                    // DB Update
                    state.ProcessState = ProcessState.Processed;
                    _orderingJobStateRepository.Update(state);

                    await _dbContext.CommitTransactionAsync(transaction);

                    // Delete Message
                    await _responseQueue.DeleteMessageAsync(retrievedMessage[0].MessageId, retrievedMessage[0].PopReceipt, cancellationToken);
                }
            }
        }
        catch
        {
            throw;
        }
    }
}

public interface IComplete
{
    Task Execute(CancellationToken cancellationToken);
}