namespace Infocom.TimeManager.WebAccess.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Dynamic;
    using System.Linq;

    using Infocom.TimeManager.WebAccess.Models;

    public class TimeRegistrationViewModel : DynamicBaseModel
    {
        

        #region Constructors and Destructors

        public TimeRegistrationViewModel()
        {
            this.TimeSheet = new TimeSheetModel();
        }

        #endregion

        #region Properties

        public ICollection<RealSpentTimeModel> RealSpentTime { get; set; }

        public TimeSheetModel TimeSheet { get; set; }

        public DateTime SelectedDate { get; set; }

        [DisplayName(@"Актуальных проектов:")]
        public int AmountOfActualProjects { get; set; }

        [DisplayName(@"Открытых задач:")]
        public int AmountOfOpenTasks { get; set; }

        public TimeSpan TotalSpentTimeInMonth { get; set; }

        public TimeSpan TotalSpentTimeInMonthFromTimeSheet { get; set; }

        #endregion

        #region Public Methods

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            Debug.WriteLine("TryGetMember");
            return base.TryGetMember(binder, out result);
        }

        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            Debug.WriteLine("TryInvokeMember");
            return base.TryInvokeMember(binder, args, out result);
        }

        public TimeSpan SpendTimeOn(DateTime selectedDate)
        {
            var spentTime = new TimeSpan(0);
            var realSpentTime = this.RealSpentTime.Where(tr => tr.StartDate == selectedDate).FirstOrDefault();
            if (realSpentTime!=null)
            {
                spentTime = realSpentTime.SpentTime;
            }
            return spentTime;
        }

        public bool isWeekend(DateTime selectedDate)
        {
            bool weekEnd = false;
            var realSpentTime = this.RealSpentTime.Where(tr => tr.StartDate == selectedDate).FirstOrDefault();
            if (realSpentTime != null)
            {
                weekEnd = realSpentTime.isWeekEnd;
            }
            return weekEnd;
        }

        public TimeSpan TotalSpentTimeByWeek()
        {
            var spentTime = new TimeSpan(this.RealSpentTime.Sum(st => st.SpentTime.Ticks));
            return spentTime;
        }

        #endregion
    }
}