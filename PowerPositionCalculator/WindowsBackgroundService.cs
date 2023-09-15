using Microsoft.Extensions.Configuration;
using Polly;
using Polly.Retry;
using Services;

using System.Configuration;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace App.WindowsService;

public sealed class WindowsBackgroundService : BackgroundService
{
    private readonly IPowerService _powerService;
    private readonly ILogger<WindowsBackgroundService> _logger;
    private readonly double _scheduleFrequency;
    private readonly IAsyncPolicy _retryPolicy;


    public WindowsBackgroundService(PowerService powerService, ILogger<WindowsBackgroundService> logger)
    {
        _powerService = powerService;
        _logger = logger;
        _scheduleFrequency = GetScheduleFrequency();
        _retryPolicy = PollyRetryPolicy.CreateRetryPolicy(logger);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Power Trade Service has started executing.");
        _logger.LogInformation("Setting Application dependencies.");


        //Create dependency output folder.         
        CommonHelper.CreateOutputDirectory();


        while (!stoppingToken.IsCancellationRequested)
        {
            //Start the process on first run.
            try
            {
                //background service logic may need retry logicm, we have configured 5 retries using Polly.
                await _retryPolicy.ExecuteAsync(async () =>
                {
                    //Call the async trade fetch
                    await FetchTradeDataAsync();
                });
            }
            catch (Exception exp)
            {

                // Handle exceptions or log them as needed
                _logger.LogError(exp, "{Message}", exp.Message);
            }

            await Task.Delay(TimeSpan.FromMinutes(_scheduleFrequency), stoppingToken);
        }

        _logger.LogInformation("Power Trade Service will stop executing.");

    }



    /// <summary>
    /// Fetch the schedule frequency from the appsettings.json and handle case if no defailt value is available.
    /// </summary>
    /// <returns></returns>
    private static double GetScheduleFrequency()
    {
        //Schedule frequency is 1 hour.
        bool result = double.TryParse(CommonHelper.ReadApplicationSettings("AppSettings:ScheduledTimeInterval"), out double frequencySchedule);
        //return default if result not true else return the value
        return !result ? 60 : frequencySchedule;
    }

    //Enumerate and build the file content
    /// <summary>
    /// Fetching data asynchronously from the PowerService and sending it to storage.
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    private async Task FetchTradeDataAsync()
    {
        _logger.LogInformation("Fetching Trade Data Asynchronously.");

        DateTime thishourdatetime = DateTime.Now;
        var tradeList = _powerService.GetTradesAsync(thishourdatetime);
        var trades = await tradeList;
        _logger.LogInformation("Finished fetching Trade Data Asynchronously.");


        Dictionary<int, PowerData> aggregate = CommonHelper.AggregateTrade(trades);
        if(aggregate!=null && aggregate.Count == 0)
        {
            _logger.LogInformation("No data found for the hour.");
            return;
        }

        _logger.LogInformation("Start writing aggregated trade data to output.");
        CommonHelper.WriteToFileAsync(CommonHelper.PrepareAggregateForExtract(aggregate), thishourdatetime);
        _logger.LogInformation($"Finished writing trade data extract. Next extract will be available in: {_scheduleFrequency} mins.");

    }

}