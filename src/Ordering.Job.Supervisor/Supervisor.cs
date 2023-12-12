namespace Ordering.Job.Supervisor;
public class Supervisor : ISupervisor
{
    private readonly OrderingJobContext _dbContext;
    private readonly ILogger<Supervisor> _logger;
    private IOrderingJobStateRepository _orderingJobRepository;
    private readonly int _retryLimit = 3;
    public Supervisor(
        OrderingJobContext dbContext,
        ILogger<Supervisor> logger,
        IOrderingJobStateRepository orderingJobRepository)
    {
        _dbContext = dbContext ?? throw new ArgumentException(nameof(dbContext));
        _logger = logger ?? throw new ArgumentException(nameof(logger));
        _orderingJobRepository = orderingJobRepository ?? throw new ArgumentException(nameof(orderingJobRepository));
    }

    public async Task Execute(CancellationToken cancellationToken)
    {
        await using var transaction = await _dbContext.BeginTransactionAsync().ConfigureAwait(false);
        try
        {
            var processingJobs = await _orderingJobRepository.FindByStateAsync(ProcessState.Processing).ConfigureAwait(false);
            foreach (var job in processingJobs)
            {
                if (job.FailureCount >= _retryLimit)
                {
                    // リトライ回数が既定値（3回）となった場合はステータスをFailedにする
                    job.ProcessState = ProcessState.Error;
                    _orderingJobRepository.Update(job);
                }
                else if (job.CompleteBy < DateTimeOffset.Now)
                {
                    // 処理の期限を過ぎた場合は失敗カウントを増やしてステータスをPendingにする
                    job.ProcessState = ProcessState.Pending;
                    job.FailureCount++;
                    job.CompleteBy = null;
                    _orderingJobRepository.Update(job);
                }
            }
            await _dbContext.CommitTransactionAsync(transaction);
        }
        catch
        {
            throw;
        }
    }
}

public interface ISupervisor
{
    Task Execute(CancellationToken cancellationToken);
}