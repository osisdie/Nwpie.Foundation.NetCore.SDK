using System;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Elasticsearch;

namespace Nwpie.Foundation.Logging.ElasticSearch
{
    public class ElasticSearchAdapter : Microsoft.Extensions.Logging.ILogger
    {
        public ElasticSearchAdapter(ElasticsearchSinkOptions option)
        {
            Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            //.Enrich.WithExceptionDetails()
                .WriteTo.Elasticsearch(option)
            .CreateLogger();
        }

        public IDisposable BeginScope<TState>(TState state) => null;

        public bool IsEnabled(LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Critical:
                    return Logger.IsEnabled(LogEventLevel.Fatal);
                case LogLevel.Debug:
                case LogLevel.Trace:
                    return Logger.IsEnabled(LogEventLevel.Debug);
                case LogLevel.Error:
                    return Logger.IsEnabled(LogEventLevel.Error);
                case LogLevel.Information:
                    return Logger.IsEnabled(LogEventLevel.Information);
                case LogLevel.Warning:
                    return Logger.IsEnabled(LogEventLevel.Warning);
                default:
                    return false;
            }
        }

        public void Log<T>(
            LogLevel logLevel,
            EventId eventId,
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
                        Logger.Information(message);
                        break;
                    case LogLevel.Warning:
                        Logger.Warning(message);
                        break;
                    default:
                        // Silence
                        break;
                }
            }
        }

        public Serilog.ILogger Logger { get; private set; }
    }
}
