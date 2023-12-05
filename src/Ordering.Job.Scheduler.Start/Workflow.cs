namespace Ordering.Job.Scheduler.Start;
public class Workflow : IWorkflow
{
    private readonly ILogger<Workflow> _logger;
    private readonly IMediator _mediator;
    public Workflow(ILogger<Workflow> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    public async Task Execute(CancellationToken cancellationToken)
    {
        var request = new WorkflowCommand();
        await _mediator.Send(request).ConfigureAwait(false);
    }
}

public interface IWorkflow
{
    Task Execute(CancellationToken cancellationToken);
}