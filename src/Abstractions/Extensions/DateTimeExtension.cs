using System;
using Nwpie.Foundation.Abstractions.Statics;

namespace Nwpie.Foundation.Abstractions.Extensions
{
    public static class DateTimeExtension
    {
        public static DateTime? ToDatetime(this string s)
        {
            if (string.IsNullOrWhiteSpace(s))
            {
                return null;
            }

            DateTime? output = null;
            if (DateTime.TryParse(s, out var tmp))
            {
                output = tmp;
            }

            return output;
        }

        public static string ToISO8601(this DateTime? src, string emptyVal = "")
        {
            return src.HasValue
                  ? src.Value.ToUniversalTime().ToString("s")
                  : emptyVal;
        }

        public static DateTime? EscapeMinValue(this DateTime dt)
        {
            return dt.ToUniversalTime() <= CommonConst.UnixBaseTime
                ? (DateTime?)null
                : dt;
        }

        public static DateTime? EscapeMinValue(this DateTime? dt)
        {
            return dt.HasValue
                ? dt.Value.EscapeMinValue()
                : dt;
        }

        public static DateTime? Truncate(this DateTime dt, TimeSpan timeSpan)
        {
            if (timeSpan == TimeSpan.Zero ||
                dt == DateTime.MinValue ||
                dt == DateTime.MaxValue)
            {
                return dt;
            }

            return dt.AddTicks(-(dt.Ticks % timeSpan.Ticks));
        }


        public static DateTime? Truncate(this DateTime? dt, TimeSpan timeSpan)
        {
            return dt.HasValue
                ? dt.Value.Truncate(timeSpan)
                : dt;
        }
    }
}
