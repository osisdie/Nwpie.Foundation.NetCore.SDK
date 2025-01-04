using System;

namespace Nwpie.Foundation.Abstractions.Extensions
{
    public static class ExceptionExtension
    {
        public static Exception GetBaseFirstException(this Exception ex)
        {
            return ex
                ?.GetBaseException()
                ?? ex;
        }

        public static string GetBaseFirstExceptionString(this Exception ex)
        {
            return (ex?.GetBaseException() ?? ex)
                ?.ToString()
                ?? string.Empty;
        }

        public static string GetBaseFirstExceptionMessage(this Exception ex)
        {
            return (ex?.GetBaseException() ?? ex)
                ?.Message
                ?? string.Empty;
        }
    }
}
