namespace Infocom.TimeManager.WebAccess.Infrastructure
{
    using System;

    public static class DateTimeExtentions
    {
        public static DateTime DateByWeekDayNumber(this DateTime date, int dayNumber)
        {
            return date.AddDays(dayNumber - 1);
        }

    }
}