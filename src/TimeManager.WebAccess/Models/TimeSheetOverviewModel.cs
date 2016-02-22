namespace Infocom.TimeManager.WebAccess.Models
{
    using System;

    public class TimeSheetOverviewModel
    {
        public string Title { get; set; }

        public TimeSpan SpentTime { get; set; }

        public DateTime Date { get; set; }
    }
}