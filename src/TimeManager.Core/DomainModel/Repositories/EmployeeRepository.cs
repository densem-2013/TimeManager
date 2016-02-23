namespace Infocom.TimeManager.Core.DomainModel.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using NHibernate.Linq;

    public class EmployeeRepository : RepositoryBase<Employee>, IEmployeeRepository
    {
        #region Public Methods

        public IEnumerable<Employee> GetByTask(Task task)
        {
            return this.GetSession().Query<Employee>().Where(e => e.Tasks.Contains(task));
        }

        #endregion

        #region Implemented Interfaces

        #region IEmployeeRepository

        public bool Exists(string login)
        {
            if (string.IsNullOrWhiteSpace(login))
            {
                throw new ArgumentNullException("login");
            }

            Employee employee;
            return this.TryGetByLogin(login, out employee);
        }

        public Employee GetByLogin(string login)
        {
            if (string.IsNullOrWhiteSpace(login))
            {
                throw new ArgumentNullException("login");
            }

            Employee result;
            if (!this.TryGetByLogin(login, out result))
            {
                throw new ObjectNotExistException(string.Format("Employee with '{0}' login is not found.", login));
            }

            return result;
        }

        #endregion

        #endregion

        #region Methods

        private bool TryGetByLogin(string login, out Employee employee)
        {
            employee = this.GetSession().Query<Employee>().Where(e => e.Login == login).FirstOrDefault();

            return employee != null;
        }

        #endregion
    }
}