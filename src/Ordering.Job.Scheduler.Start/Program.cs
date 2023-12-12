var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddHostedService<Worker>();

        var dbConnectionString = hostContext.Configuration.GetConnectionString(nameof(OrderingJobContext));
        services.AddDbContext<OrderingJobContext>(
            x => x.UseSqlServer(dbConnectionString));

        var saConnectionString = hostContext.Configuration.GetValue("AZURE_STORAGE_CONNECTION_STRING", "");
        var queueName = hostContext.Configuration.GetValue("AZURE_STORAGE_REQUEST_QUEUE_NAME", "req");
        services.AddScoped(s => new QueueClient(saConnectionString, queueName));

        services.AddScoped<IOrderingJobStateRepository, OrderingJobStateRepository>();

        services.AddScoped<IWorkflow, Workflow>();
    })
    .Build();

await host.StartAsync().ConfigureAwait(false);