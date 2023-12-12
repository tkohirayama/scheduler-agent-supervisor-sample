namespace Ordering.Job.Scheduler.Start;

public class Worker : IHostedService
{
    private readonly ILogger<Worker> _logger;
    private readonly IHostApplicationLifetime _appLifetime;
    private readonly IServiceProvider _serviceProvider;
    public Worker(ILogger<Worker> logger, IHostApplicationLifetime appLifetime,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _appLifetime = appLifetime;
        _serviceProvider = serviceProvider;

        _appLifetime.ApplicationStarted.Register(OnStarted);
        _appLifetime.ApplicationStopped.Register(OnStopped);
    }
    void OnStarted()
    {
        _logger.LogInformation("Started Ordering.Job.Scheduler.Start");
    }

    void OnStopped()
    {
        _logger.LogInformation("Stopped Ordering.Job.Scheduler.Start");
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var workflow = scope.ServiceProvider.GetRequiredService<IWorkflow>();
            await workflow.Execute(cancellationToken).ConfigureAwait(false);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error Ordering.Job.Scheduler.Start");
            throw;
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}