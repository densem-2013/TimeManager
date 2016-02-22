namespace Infocom.TimeManager.Core.DomainModel
{
    using System.Collections.Generic;

    using NHibernate.Validator.Constraints;

    [PersistedEntity]
    public class Department : DomainObject
    {
        #region Constructors and Destructors

        public Department()
        {
            this.Employees = new List<Employee>();
        }

        #endregion

        #region Properties

        [NotNull]
        public virtual string Name { get; set; }

        [NotNull]
        public virtual string ShortName { get; set; }

        public virtual string Email { get; set; }

        private Employee manager;

        public virtual Employee Manager
        {
            get
            {
                return this.manager;
            }
            set
            {
                if (value != null)
                {
                    value.Department = this;
                }

                this.manager = value;
            }
        }

        public virtual ICollection<Employee> Employees { get; set; }

        public virtual int RequestPermission { get; set; }

        public virtual int TimeregPermission { get; set; }

        #endregion
    }
}