namespace Infocom.TimeManager.WebAccess.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Threading;

    using Infocom.TimeManager.WebAccess.Extensions;

    public class TaskModel : DynamicBaseModel
    {
        private long _employeeID = 0;

        #region Constructors and Destructors

        public TaskModel()
        {
            this.AssignedEmployeeIDs = new List<long>();
            this.TimeRecords = new List<TimeRecordModel>();
            this.RegistrationDate = DateTime.Now;
        }

        #endregion

        #region Properties

        public long ParentTaskID { get; set; }

        [DisplayName(@"Наименование")]
        [Required(ErrorMessage = @"Не задано")]
        public string Name { get; set; }

        [UIHint("Description")]
        [DisplayName(@"Описание")]
        public string Description { get; set; }

        [DisplayName(@"Статус задачи")]
        public string CurrentStatusName { get; set; }

        [DisplayName(@"Статус задачи")]
        public long TaskStatusID { get; set; }

        // [UIHint("Time")]
        // [DisplayName(@"Запланированое время")]
        // [Required(ErrorMessage = @"Не задано")]
        // public DateTime? TimeBudget { get; set; }

        [DisplayName(@"Дата начала")]
        public DateTime? StartDate { get; set; }

        [DisplayName(@"Дата окончания")]
        public DateTime? FinishDate { get; set; }

        [DisplayName(@"Дата регистрации")]
        public DateTime? RegistrationDate { get; set; }

        public string ProjectName { get; set; }

        public string ProjectCode { get; set; }

        public long ProjectID { get; set; }

        [DisplayName(@"Назначеные сотрудники")]
        public IEnumerable<long> AssignedEmployeeIDs { get; set; }

        [DisplayName(@"Назначеные сотрудники")]
        public string AssignedEmployeeNames { get; set; }

        public ICollection<TimeRecordModel> TimeRecords { get; set; }

        public TimeSpan TotalSpentTime
        {
            get
            {
                return new TimeSpan(this.TimeRecords.Sum(tr => tr.SpentTime.Ticks));
            }
        }

        #endregion

        public void SetEmployeeID(long value)
        {
            _employeeID = value;
        }

        public TimeSpan SpendTimeByWeek(DateTime selectedDate)
        {
            var startDate = selectedDate.FirstDayOfWeek();
            var finishDate = startDate.AddDays(7).AddMilliseconds(-1);
            var timeRecordsForSelectedDate = this.TimeRecords.Where(tr => tr.StartDate >= startDate && tr.StartDate < finishDate && tr.EmployeeID == _employeeID);
            return new TimeSpan(timeRecordsForSelectedDate.Sum(tr => tr.SpentTime.Ticks));
        }

        public TimeSpan SpendTimeOn(DateTime selectedDate)
        {
            var timeRecordsForSelectedDate = this.TimeRecords.Where(tr => tr.StartDate == selectedDate && tr.EmployeeID == _employeeID);
            return new TimeSpan(timeRecordsForSelectedDate.Sum(tr => tr.SpentTime.Ticks));
        }

        

        public override bool TryGetMember(System.Dynamic.GetMemberBinder binder, out object result)
        {
            result = TimeSpan.FromHours(0); //todo: recheck this
            return true;
        }
    }
}