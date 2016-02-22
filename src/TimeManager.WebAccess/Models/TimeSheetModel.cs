namespace Infocom.TimeManager.WebAccess.Models
{
    using System;
    using System.Collections.Generic;

    using Infocom.TimeManager.Core.DomainModel;

    using System.Linq;

    public class TimeSheetModel : BaseModel
    {
        public TimeSheetModel()
        {
            this.Tasks = new List<TaskModel>();
        }

        public ICollection<TaskModel> Tasks { get; set; }

        public EmployeeModel Employee { get; set; }

        public DateTime Date { get; set; }

        public TimeSheetType Type { get; set; }

        public TimeSpan TotalSpentTimeByWeek
        {
            get
            {
                if (Date == DateTime.MinValue)
                {
                    throw new ArgumentException("Date is not been set");
                }
                return new TimeSpan(Tasks.Sum(t => t.SpendTimeByWeek(this.Date).Ticks));
            }
        }

        public TimeSpan SpendTimeOn(DateTime day)
        {
            var fromDate = day.Date;
            var toDate = day.Date.AddDays(1).AddMilliseconds(-1);

            var spendTime = (from t in Tasks
                             from tr in t.TimeRecords
                             where tr.StartDate >= fromDate && tr.StartDate < toDate && tr.EmployeeID == Employee.ID
                             select tr.SpentTime.Ticks).Sum();

            return new TimeSpan(spendTime);
        }
    }
}