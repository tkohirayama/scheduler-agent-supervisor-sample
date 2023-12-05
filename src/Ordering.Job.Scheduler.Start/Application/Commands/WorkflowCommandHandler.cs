namespace Ordering.Job.Scheduler.Start.Application.Commands;

public class WorkflowCommandHandler : IRequestHandler<WorkflowCommand, bool>
{
    private IOrderingJobStateRepository _orderingJobRepository;
    private readonly QueueClient _remoteServiceQueueClient;

    public WorkflowCommandHandler(IOrderingJobStateRepository orderingJobRepository, QueueClient remoteServiceQueueClient)
    {
        _orderingJobRepository = orderingJobRepository ?? throw new ArgumentException(nameof(orderingJobRepository));
        _remoteServiceQueueClient = remoteServiceQueueClient ?? throw new ArgumentException(nameof(remoteServiceQueueClient));
    }

    public async Task<bool> Handle(WorkflowCommand request, CancellationToken cancellationToken)
    {
        var pendingOrderingJobs = await _orderingJobRepository.FindByStateAsync(ProcessState.Pending).ConfigureAwait(false);
        foreach(var job in pendingOrderingJobs)
        {
            // TODO: 事前処理コマンドに移動
            job.ProcessState = ProcessState.Processing;
            job.CompleteBy = DateTimeOffset.UtcNow.AddMinutes(10);
            job.LockedBy = "Worker1";
            _orderingJobRepository.Update(job);

            // Workflow Something

            // Send Message To Remote Service Agent
            var message = new BinaryData(job);
            await _remoteServiceQueueClient.SendMessageAsync(message).ConfigureAwait(false);
        }
        await _orderingJobRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken).ConfigureAwait(false);
        return true;
    }
}
