var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddHostedService<Worker>();

        var dbConnectionString = hostContext.Configuration.GetConnectionString(nameof(OrderingJobContext));
        services.AddDbContext<OrderingJobContext>(
            x => x.UseSqlServer(dbConnectionString));

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblyContaining(typeof(Program));

            cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
            cfg.AddOpenBehavior(typeof(ValidatorBehavior<,>));
            cfg.AddOpenBehavior(typeof(TransactionBehavior<,>));
        });

        services.AddSingleton<IValidator<SupervisorCommand>, SupervisorCommandValidator>();

        services.AddScoped<IOrderingJobStateRepository, OrderingJobStateRepository>();

        services.AddScoped<ISupervisor, Supervisor>();
    })
    .Build();

await host.RunAsync();