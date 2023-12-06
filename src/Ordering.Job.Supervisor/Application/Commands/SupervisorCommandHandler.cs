namespace Ordering.Job.Supervisor.Application.Commands;

public class SupervisorCommandHandler : IRequestHandler<SupervisorCommand, bool>
{
    private IOrderingJobStateRepository _orderingJobRepository;
    private readonly int _retryLimit;
    public SupervisorCommandHandler(IOrderingJobStateRepository orderingJobRepository, IConfiguration configuration)
    {
        _orderingJobRepository = orderingJobRepository ?? throw new ArgumentException(nameof(orderingJobRepository));
        _retryLimit = configuration.GetValue("Supervisor:RetryLimit", 3);
    }

    public async Task<bool> Handle(SupervisorCommand request, CancellationToken cancellationToken)
    {
        var processingJobs = await _orderingJobRepository.FindByStateAsync(ProcessState.Processing).ConfigureAwait(false);
        foreach (var job in processingJobs)
        {
            if (job.FailureCount >= _retryLimit)
            {
                job.ProcessState = ProcessState.Error;
                _orderingJobRepository.Update(job);
            }
            else if (job.CompleteBy < DateTimeOffset.Now)
            {
                // retry settting
                job.ProcessState = ProcessState.Pending;
                job.FailureCount++;
                job.CompleteBy = null;
                _orderingJobRepository.Update(job);
            }
        }
        await _orderingJobRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken).ConfigureAwait(false);
        return true;
    }
}
