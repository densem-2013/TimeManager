using System;

namespace Infocom.TimeManager.WebAccess.Extensions
{
    using System.Globalization;

    public static class DateExtensions
    {
        public static DateTime FirstDayOfMonth(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, 1);
        }

        public static DateTime LastDayOfMonth(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, FirstDayOfMonth(date).AddMonths(1).AddDays(-1).Day);
        }

        public static DateTime FirstDayOfWeek(this DateTime dateTime)
        { 
            var firstDayOfWeek = dateTime.AddDays(
                    (dateTime.DayOfWeek == DayOfWeek.Sunday ? 6 : dateTime.DayOfWeek - DayOfWeek.Monday) * -1);
            return new DateTime(firstDayOfWeek.Year,firstDayOfWeek.Month,firstDayOfWeek.Day);
        }

        public static int WeekNumber(this DateTime target)
        {
            return CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(target, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        }

        public static string ToFormatedTime(this TimeSpan dateTime)
        {
            return dateTime.Ticks.ToFormatedTime();
        }

        public static string ToFormatedTime(this long ticks)
        {
            return string.Format("{0:00}:{1:00}", Math.Floor(new TimeSpan(ticks).TotalHours), new TimeSpan(ticks).Minutes);
        }
    }
}