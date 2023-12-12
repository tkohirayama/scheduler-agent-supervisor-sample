var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddHostedService<Worker>();

        var dbConnectionString = hostContext.Configuration.GetConnectionString(nameof(OrderingJobContext));
        services.AddDbContext<OrderingJobContext>(
            x => x.UseSqlServer(dbConnectionString));

        services.AddScoped<IOrderingJobStateRepository, OrderingJobStateRepository>();

        services.AddScoped<ISupervisor, Supervisor>();
    })
    .Build();

await host.StartAsync().ConfigureAwait(false);