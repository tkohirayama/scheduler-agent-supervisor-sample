using Ordering.Job.Scheduler.Start.Application.Commands;

namespace Ordering.Job.Scheduler.Start;

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
        _logger.LogInformation("Started Ordering.Job.Scheduler.Start");
    }

    void OnStopped()
    {
        _logger.LogInformation("Stopped Ordering.Job.Scheduler.Stopped");
    }


    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var workflow = scope.ServiceProvider.GetRequiredService<IWorkflow>();
                await workflow.Execute(stoppingToken).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error Ordering.Job.Scheduler.Complete");
                throw;
            }
            await Task.Delay(1000).ConfigureAwait(false);
        }
    }
}