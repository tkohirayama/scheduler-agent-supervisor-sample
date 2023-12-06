namespace Ordering.Job.Supervisor;
public class Supervisor : ISupervisor
{
    private readonly ILogger<Supervisor> _logger;
    private readonly IMediator _mediator;
    public Supervisor(ILogger<Supervisor> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    public async Task Execute(CancellationToken cancellationToken)
    {
        var request = new SupervisorCommand();
        await _mediator.Send(request, cancellationToken).ConfigureAwait(false);
    }
}

public interface ISupervisor
{
    Task Execute(CancellationToken cancellationToken);
}