using System;
using Microsoft.Extensions.Logging;

namespace Nwpie.Foundation.Abstractions.Logging
{
    public static class LogMgr
    {
        public static ILogger<T> CreateLogger<T>() =>
            LoggerFactory.CreateLogger<T>();

        public static ILogger CreateLogger(Type type) =>
            LoggerFactory.CreateLogger(type);

        public static ILoggerFactory LoggerFactory = new LoggerFactory();
    }
}
