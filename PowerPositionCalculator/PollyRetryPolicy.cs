using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Polly;
using Polly.Retry;

namespace App.WindowsService
{

    public static class PollyRetryPolicy
    {
        public static IAsyncPolicy CreateRetryPolicy(ILogger<WindowsBackgroundService> logger)
        {

            return Policy
            .Handle<Exception>() // Handle any exception type
            .RetryAsync(3, (exception, retryCount, context) =>
            {
                // Customize and log the retry message
                string message = $"Retry attempt {retryCount} failed due to {exception.Message}";
                logger.LogError(message);
            });           
        }
    }

}
