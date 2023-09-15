using App.WindowsService;
using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.Logging.EventLog;
using Services;

IHostBuilder builder = Host.CreateDefaultBuilder(args)
    .UseWindowsService(options =>
    {
        options.ServiceName = "Petroineos Power Trades Fetch Service";
    })
    .ConfigureServices((context, services) =>
    {
        LoggerProviderOptions.RegisterProviderOptions<
            EventLogSettings, EventLogLoggerProvider>(services);
        services.AddSingleton<PowerService>();
        services.AddHostedService<WindowsBackgroundService>();

        //Register the PollyRetryPolicy as a singleton
        //services.AddSingleton<PollyRetryPolicy>();
        IConfigurationRoot configuration = new ConfigurationBuilder()
        .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
    .Build(); 

    });

IHost host = builder.Build();
host.Run();