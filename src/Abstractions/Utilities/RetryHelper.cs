using System;
using System.Threading.Tasks;
using Nwpie.Foundation.Abstractions.Logging;
using Microsoft.Extensions.Logging;

namespace Nwpie.Foundation.Abstractions.Utilities
{
    public static class RetryHelper
    {
        static RetryHelper()
        {
            m_Logger = LogMgr.CreateLogger(typeof(RetryHelper));
        }

        public static async Task RetryOnException(int retryTimes, TimeSpan delay, Func<Task> exec)
        {
            var attempts = 0;
            do
            {
                try
                {
                    await exec();
                    break; // Success then exit the loop.
                }
                catch (Exception ex)
                {
                    if (attempts++ >= retryTimes)
                    {
                        throw;
                    }

                    m_Logger.LogWarning(ex, $"Exception caught on attempt {attempts} - will retry after delay {delay}. ");
                    if (delay.TotalMilliseconds > 1)
                    {
                        await Task.Delay(delay);
                    }
                }
            } while (true);
        }

        private static readonly ILogger m_Logger;
    }
}
