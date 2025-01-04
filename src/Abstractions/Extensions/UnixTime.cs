using System;

namespace Nwpie.Foundation.Abstractions.Extensions
{
    public static class UnixTime
    {
        public static DateTimeOffset Epoch = new DateTimeOffset(new DateTime(1970, 1, 1), TimeSpan.Zero);

        public static double FromLocal(DateTime time) =>
            (time.ToUniversalTime() - Epoch).TotalSeconds;

        public static double FromUtc(DateTime time) =>
            (time - Epoch).TotalSeconds;

        public static DateTimeOffset ToDateTimeOffset(double seconds) =>
            Epoch.AddSeconds(seconds);

        public static DateTime ToUtcDateTime(double seconds) =>
            ToDateTimeOffset(seconds).UtcDateTime;
    }
}
