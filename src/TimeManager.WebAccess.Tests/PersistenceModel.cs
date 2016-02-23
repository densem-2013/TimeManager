namespace Infocom.TimeManager.WebAccess.Tests
{
    using System;
    using System.Collections.Generic;

    using Infocom.TimeManager.Core.DomainModel;

    public class PersistentModel
    {
        #region Constructors and Destructors

        public PersistentModel(IEnumerable<Project> projects, IEnumerable<Employee> employees, TimeSheet timeSheet)
        {
            if (projects == null)
            {
                throw new ArgumentNullException("projects");
            }

            if (employees == null)
            {
                throw new ArgumentNullException("employees");
            }

            if (timeSheet == null)
            {
                throw new ArgumentNullException("timeSheet");
            }

            this.Projects = projects;
            this.Employees = employees;
            this.TimeSheet = timeSheet;
        }

        #endregion

        #region Properties

        public IEnumerable<Project> Projects { get; private set; }

        public IEnumerable<Employee> Employees { get; private set; }

        public TimeSheet TimeSheet { get; private set; }

        #endregion
    }
}