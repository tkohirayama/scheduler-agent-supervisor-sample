using SchedulerAgentSupervisor.Scheduler.Receiver;
using Azure.Storage.Queues;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();
