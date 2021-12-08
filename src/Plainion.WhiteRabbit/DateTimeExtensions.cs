using System;
using System.Globalization;

namespace Plainion.WhiteRabbit
{
    public static class DateTimeExtensions
    {
        public static DateTime GetBeginOfWeek(this DateTime date)
        {
            while (date.DayOfWeek != DayOfWeek.Monday)
            {
                date = date.AddDays(-1);
            }

            return date;
        }

        public static DateTime GetEndOfWeek(this DateTime date)
        {
            while (date.DayOfWeek != DayOfWeek.Sunday)
            {
                date = date.AddDays(1);
            }

            return date;
        }

        public static string ToReportString(this TimeSpan span)
        {
            if (span == TimeSpan.Zero)
            {
                return "-";
            }

            return string.Format(CultureInfo.InvariantCulture, "{0,2:00}:{1,2:00}", span.Days * 24 + span.Hours, span.Minutes);
        }
    }
}