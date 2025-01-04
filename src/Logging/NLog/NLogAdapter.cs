using System;
using System.IO;
using NLog;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace Nwpie.Foundation.Logging.NLog
{
    public class NLogAdapter : Microsoft.Extensions.Logging.ILogger
    {
        public NLogAdapter(string loggerName, FileInfo fileInfo)
        {
            LogManager.LoadConfiguration(fileInfo.FullName);
            Logger = LogManager.GetLogger(loggerName);
        }

        public IDisposable BeginScope<T>(T state) => null;

        public bool IsEnabled(LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Critical:
                    return Logger.IsFatalEnabled;
                case LogLevel.Debug:
                case LogLevel.Trace:
                    return Logger.IsDebugEnabled;
                case LogLevel.Error:
                    return Logger.IsErrorEnabled;
                case LogLevel.Information:
                    return Logger.IsInfoEnabled;
                case LogLevel.Warning:
                    return Logger.IsWarnEnabled;
                default:
                    return false;
            }
        }

        public void Log<T>(
            LogLevel logLevel,
            Microsoft.Extensions.Logging.EventId eventId,
            T state,
            Exception exception,
            Func<T, Exception, string> formatter)
        {
            if (false == IsEnabled(logLevel))
            {
                return;
            }

            if (null == formatter)
            {
                throw new ArgumentNullException(nameof(formatter));
            }

            var message = formatter(state, exception);
            if (string.IsNullOrWhiteSpace(message))
            {
                message = exception?.ToString();
            }

            if (false == string.IsNullOrWhiteSpace(message))
            {
                switch (logLevel)
                {
                    case LogLevel.Critical:
                        Logger.Fatal(message);
                        break;
                    case LogLevel.Debug:
                    case LogLevel.Trace:
                        Logger.Debug(message);
                        break;
                    case LogLevel.Error:
                        Logger.Error(message);
                        break;
                    case LogLevel.Information:
                        Logger.Info(message);
                        break;
                    case LogLevel.Warning:
                        Logger.Warn(message);
                        break;
                    default:
                        // Silence
                        break;
                }
            }
        }

        public ILogger Logger { get; private set; }
    }
}
