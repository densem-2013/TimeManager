namespace Infocom.TimeManager.WebAccess.Models
{
    using System;

    public class TimeSheetTaskAllocation
    {
        public DateTime Date { get; set; }

        public TimeSpan AmountOfTimeSpent { get; set; }
    }
}