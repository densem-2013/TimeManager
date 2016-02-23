namespace Infocom.TimeManager.Core.DomainModel
{
    using System;
    using System.Collections.Generic;

    using NHibernate.Validator.Constraints;

    [PersistedEntity]
    public class Project : DomainObject
    {
        #region Constants and Fields

        private Status _currentStatus;

        private ICollection<Task> _tasks;

        private ICollection<Task> _tasksWrapper;

        private ICollection<Employee> _employees;

        private ICollection<Employee> _employeesWrapper;
        
        #endregion

        #region Constructors and Destructors

        public Project()
        {
            this._tasks = new List<Task>();
            this._employees = new List<Employee>();
        }

        #endregion

        #region Properties

        [NotNull]
        public virtual string Name { get; set; }

        [NotNull]
        public virtual string Code { get; set; }

        public virtual decimal? Rate { get; set; }

        public virtual string Description { get; set; }

      //  public virtual DateTime StartDate { get; set; }

        [NotNull]
        public virtual ProjectType ProjectType { get; set; }

        [NotNull]
        public virtual ProjectPriority ProjectPriority { get; set; }

        [NotNull]
        public virtual Phase Phase { get; set; }

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
                    var applicableType = DomailEntityType.Project;
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

        public virtual ICollection<Task> Tasks
        {
            get
            {
                return this._tasksWrapper ??
                       (this._tasksWrapper =
                        new LinkCollection<Task>(this._tasks, t => t.Project = this, t => t.Project = null));
            }
        }

        public virtual ICollection<Employee> Employees
        {
            get
            {
                return this._employeesWrapper ??
                       (this._employeesWrapper =
                        new LinkCollection<Employee>(this._employees, e => e.Projects.Add(this), e => e.Projects.Remove(this)));
            }
        }

        #endregion

        public virtual void SetEmployees(List<Employee> assignedEmployees)
        {
            this.Employees.Clear();
            assignedEmployees.ForEach(e => this.Employees.Add(e));
            
        }
    }
}