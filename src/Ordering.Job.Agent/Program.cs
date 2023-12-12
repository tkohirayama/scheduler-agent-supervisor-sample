using Ordering.Job.Agent;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddHostedService<Worker>();
        services.AddScoped<IRemoteService, RemoteServiceMock>();
    })
    .Build();

await host.RunAsync();
