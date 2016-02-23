namespace Infocom.TimeManager.Core.DomainModel
{
    using System;
    using System.Collections.Generic;

    using NHibernate.Validator.Constraints;

    [PersistedEntity]
    public class Task : DomainObject
    {
        #region Constants and Fields

        private ICollection<Employee> _assignedEmployees;

        private ICollection<Employee> _assignedEmployeesCollection;

        private ICollection<TimeSheet> _timeSheetsWrapper;

        private ICollection<TimeSheet> _timeSheets;

        private Status _currentStatus;

        private Project _project;

        #endregion

        #region Constructors and Destructors

        public Task()
        {
            this.TimeRecords = new List<TimeRecord>();
            this._assignedEmployees = new List<Employee>();
            this._timeSheets = new List<TimeSheet>();
        }

        #endregion

        #region Properties

        [NotNull]
        public virtual string Name { get; set; }

        public virtual ICollection<Employee> AssignedEmployees
        {
            get
            {
                return this._assignedEmployeesCollection ??
                       (this._assignedEmployeesCollection =
                        new LinkCollection<Employee>(
                            this._assignedEmployees, e => e.Tasks.Add(this), e => e.Tasks.Remove(this)));
            }
        }

        public virtual string Description { get; set; }

        public virtual Status CurrentStatus
        {
            get
            {
                return this._currentStatus;
            }

            set
            {
                if (value != null)
                {
                    var applicableType = DomailEntityType.Task;
                    if (value.ApplicableTo != applicableType)
                    {
                        throw new InvalidOperationException(
                            String.Format(
                                "'{0}' is not applicable, you can assign only {1} type",
                                value.ApplicableTo,
                                applicableType));
                    }
                }

                this._currentStatus = value;
            }
        }

        public virtual Task ParentTask { get; set; }

        public virtual ICollection<TimeRecord> TimeRecords { get; set; }

        // [NotNull]
        public virtual Project Project
        {
            get
            {
                return this._project;
            }

            set
            {
                if (!value.Tasks.Contains(this))
                {
                    value.Tasks.Add(this);
                }

                this._project = value;
            }
        }

        public virtual TimeSpan? TimeBudget { get; set; }

        public virtual DateTime? StartDate { get; set; }

        public virtual DateTime? FinishDate { get; set; }

        public virtual DateTime? RegistrationDate { get; set; }

        public virtual ICollection<TimeSheet> TimeSheets
        {
            get
            {
                return this._timeSheetsWrapper ??
                       (this._timeSheetsWrapper =
                        new LinkCollection<TimeSheet>(
                            this._timeSheets, e => e.Tasks.Add(this), e => e.Tasks.Remove(this)));
            }
        }
        #endregion

        public virtual void SetEmployees(List<Employee> assignedEmployees)
        {
            this.AssignedEmployees.Clear();
            assignedEmployees.ForEach(e => this.AssignedEmployees.Add(e));
        }
    }
}