namespace Infocom.TimeManager.Core.DomainModel
{
    using System.Collections.Generic;
    using System.Linq;

    using NHibernate.Validator.Constraints;
    using System;

    [PersistedEntity]
    public class Employee : DomainObject
    {
        #region Constants and Fields

        private ICollection<Task> _tasks;

        private ICollection<Task> _tasksWrapper;

        private ICollection<Project> _projects;

        private ICollection<Project> _projectsWrapper;

        #endregion

        #region Constructors and Destructors

        public Employee()
        {
            this._tasks = new List<Task>();
            this._projects = new List<Project>();
            this.Subordinates = new List<Employee>();
        }

        #endregion

        #region Properties

        [NotNull]
        public virtual string FirstName { get; set; }

        [NotNull]
        public virtual string LastName { get; set; }

        [NotNull]
        public virtual string PatronymicName { get; set; }

        [NotNull]
        public virtual string Login { get; set; }

        public virtual string Email { get; set; }

        public virtual int ApprovePermission { get; set; }

        public virtual int? HumanResource { get; set; }

        public virtual Employee Manager { get; set; }

        public virtual ICollection<Employee> Subordinates { get; set; }

        public virtual ICollection<Task> Tasks
        {
            get
            {
                return this._tasksWrapper ??
                       (this._tasksWrapper =
                        new LinkCollection<Task>(
                            this._tasks, e => e.AssignedEmployees.Add(this), e => e.AssignedEmployees.Remove(this)));
            }
        }

        public virtual ICollection<Project> Projects
        {
            get
            {
                return this._projectsWrapper ??
                       (this._projectsWrapper =
                        new LinkCollection<Project>(
                            this._projects, e => e.Employees.Add(this), e => e.Employees.Remove(this)));
            }
        }

        public virtual string ShortName
        {
            get
            {
                if (string.IsNullOrWhiteSpace(this.FirstName) || string.IsNullOrWhiteSpace(this.PatronymicName))
                {
                    return string.Empty;
                }

                return string.Format(
                    "{0} {1}. {2}.", this.LastName, this.FirstName.First(), this.PatronymicName.First());
            }
        }

        public virtual ICollection<EmployeeRate> EmployeeRates { get; set; }

        public virtual Department Department { get; set; }

        public virtual decimal? Rate { get; set; }

        public virtual DateTime? HireDate { get; set; }

        public virtual DateTime? FireDate { get; set; }

        #endregion
    }
}