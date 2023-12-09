using Ordering.Job.Scheduler.Complete;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddHostedService<Worker>();

        var dbConnectionString = hostContext.Configuration.GetConnectionString(nameof(OrderingJobContext));
        services.AddDbContext<OrderingJobContext>(
            x => x.UseSqlServer(dbConnectionString));

        var saConnectionString = hostContext.Configuration.GetValue("AZURE_STORAGE_CONNECTION_STRING", "");
        var queueName = hostContext.Configuration.GetValue("AZURE_STORAGE_RESPONSE_QUEUE_NAME", "res");
        services.AddScoped(s => new QueueClient(saConnectionString, queueName));

        services.AddScoped<IOrderingJobStateRepository, OrderingJobStateRepository>();

        services.AddScoped<IComplete, Complete>();
    })
    .Build();

await host.RunAsync();
