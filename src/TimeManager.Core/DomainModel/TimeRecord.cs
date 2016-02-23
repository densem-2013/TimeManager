namespace Infocom.TimeManager.Core.DomainModel
{
    using System;
 
    using Infocom.TimeManager.Core.DomainModel.Validators;

    using NHibernate.Validator.Constraints;

    [PersistedEntity]
    public class TimeRecord : DomainObject
    {
        #region Constructors and Destructors

        public TimeRecord()
        {
        }

        public TimeRecord(Employee employee, Task task, DateTime startDate, TimeSpan spentTime)
        {
            this.Employee = employee;
            this.Task = task;
            this.StartDate = startDate;
            this.SpentTime = spentTime;
        }

        #endregion

        #region Properties

        [NotNull]
        public virtual Employee Employee { get; set; }

        [NotNull]
        public virtual Task Task { get; set; }

        [MaximumDateTime(MaximumValue = "2100.01.01")]
        [MinimumDateTime(MinimumValue = "2000.01.01")]
        public virtual DateTime? StartDate { get; set; }

        public virtual TimeSpan? SpentTime { get; set; }

        public virtual string Description { get; set; }

        #endregion
    }
}