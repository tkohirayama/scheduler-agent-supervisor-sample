namespace Ordering.Job.Supervisor;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IHostApplicationLifetime _appLifetime;
    private readonly IServiceProvider _serviceProvider;
    public Worker(ILogger<Worker> logger, IHostApplicationLifetime appLifetime, IMediator mediator,
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
        _logger.LogInformation("Started Ordering.Job.Supervisor");
    }

    void OnStopped()
    {
        _logger.LogInformation("Stopped Ordering.Job.Supervisor");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var supervisor = scope.ServiceProvider.GetRequiredService<ISupervisor>();
                await supervisor.Execute(stoppingToken).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error Ordering.Job.Supervisor");
                throw;
            }
            await Task.Delay(1000, stoppingToken);
        }
    }
}
