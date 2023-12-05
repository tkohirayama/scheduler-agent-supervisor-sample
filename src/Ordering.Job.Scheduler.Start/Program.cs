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

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblyContaining(typeof(Program));

            cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
            // cfg.AddOpenBehavior(typeof(ValidatorBehavior<,>));
            // cfg.AddOpenBehavior(typeof(TransactionBehavior<,>));
        });

        services.AddSingleton<IValidator<WorkflowCommand>, WorkflowCommandValidator>();

        services.AddScoped<IOrderingJobStateRepository, OrderingJobStateRepository>();

        services.AddScoped<IWorkflow, Workflow>();
    })
    .Build();

await host.RunAsync();
