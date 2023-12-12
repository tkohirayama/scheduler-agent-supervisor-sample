using System.Text.Json;
using Ordering.Job.Domain.Messages;

namespace Ordering.Job.Scheduler.Start;
public class Workflow : IWorkflow
{
    private readonly OrderingJobContext _dbContext;
    private readonly IOrderingJobStateRepository _orderingJobRepository;
    private readonly QueueClient _remoteServiceQueueClient;
    private readonly ILogger<Workflow> _logger;
    public Workflow(
        OrderingJobContext dbContext,
        IOrderingJobStateRepository orderingJobRepository,
        QueueClient remoteServiceQueueClient,
        ILogger<Workflow> logger)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _remoteServiceQueueClient = remoteServiceQueueClient ?? throw new ArgumentException(nameof(remoteServiceQueueClient));
        _orderingJobRepository = orderingJobRepository ?? throw new ArgumentException(nameof(orderingJobRepository));
        _logger = logger;
    }

    public async Task Execute(CancellationToken cancellationToken)
    {
        await using var transaction = await _dbContext.BeginTransactionAsync().ConfigureAwait(false);
        try
        {
            var pendingOrderingJobs = await _orderingJobRepository.FindByStateAsync(ProcessState.Pending).ConfigureAwait(false);
            foreach (var job in pendingOrderingJobs)
            {
                job.ProcessState = ProcessState.Processing;
                job.CompleteBy = DateTimeOffset.UtcNow.AddMinutes(10); // 10分後を期限に設定
                job.LockedBy = "Worker1"; // TODO: LockedBy Code
                _orderingJobRepository.Update(job);

                // Workflow Something

                // Send Message To Agent
                var requestModel = new RequestModel
                {
                    OrderId = job.Id,
                };
                await _remoteServiceQueueClient.SendMessageAsync(JsonSerializer.Serialize(requestModel)).ConfigureAwait(false);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
        finally
        {
            await _dbContext.CommitTransactionAsync(transaction).ConfigureAwait(false);
        }
    }
}

public interface IWorkflow
{
    Task Execute(CancellationToken cancellationToken);
}