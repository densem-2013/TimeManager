namespace Infocom.TimeManager.Core.Services
{
    using System;
    using System.Linq;
    using System.Security.Authentication;
    using System.Security.Principal;
    using System.ServiceModel;
    using System.Threading;

    using Infocom.TimeManager.Core.DomainModel;
    using Infocom.TimeManager.Core.Services.Dto;

    using NHibernate;

    using ObjectNotFoundException = Infocom.TimeManager.Core.DomainModel.Repositories.ObjectNotExistException;

    [NHibernateContext]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class TaskManagement : ITaskManagement, IDisposable
    {
        #region Constants and Fields

        private DtoAssembler _dtoAssembler;

        #endregion

        #region Constructors and Destructors

        public TaskManagement()
        {
            this._dtoAssembler = new DtoAssembler();
        }

        #endregion

        #region Properties

        public ISession Session
        {
            get
            {
                return NHibernateContext.Current().Session;
            }
        }

        #endregion

        #region Implemented Interfaces

        #region IDisposable

        public void Dispose()
        {
            try
            {
                NHibernateContext.Current().Session.Dispose();
            }
            catch (Exception ex)
            {
            }
        }

        #endregion

        #region ITaskManagement

        public TimeRecordDto BookTime(long taskID, DateTime startDate, TimeSpan spentTime)
        {
            if (taskID == default(long))
            {
                throw new ArgumentException(string.Format("taskID argument should not be equals to '{0}'", taskID));
            }

            if (startDate == default(DateTime))
            {
                throw new ArgumentException(
                    string.Format("startDate argument should not be equals to '{0}'", startDate));
            }

            if (spentTime == default(TimeSpan))
            {
                throw new ArgumentException(
                    string.Format("spentTime argument should not be equals to '{0}'", spentTime));
            }

            this.CheckForUserRole();

            var login = this.GetLogin();
            var task = this.Session.QueryOver<Task>().Where(t => t.ID == taskID).FutureValue<Task>();
            var employee = this.Session.QueryOver<Employee>().Where(e => e.Login == login).FutureValue();

            if (employee.Value == null)
            {
                throw new ObjectNotFoundException(String.Format("Employee with login '{0}' is not registered.", login));
            }

            if (task.Value == null)
            {
                throw new ObjectNotFoundException(String.Format("Task with ID='{0}' is not found.", taskID));
            }

            var timeRecord = new TimeRecord(employee.Value, task.Value, startDate, spentTime);

            this.Session.SaveOrUpdate(timeRecord);

            return this._dtoAssembler.CreateTimeRecordDto(timeRecord);
        }

        public TaskDto[] GetWorkItems()
        {
            this.CheckForUserRole();
            var login = this.GetLogin();

            var tasks =
                this.Session.QueryOver<Task>().JoinQueryOver<Employee>(t => t.AssignedEmployees).Where(
                    e => e.Login == login).List();

            var result = tasks.Select(t => this._dtoAssembler.CreateTaskDto(t));

            return result.ToArray();
        }

        #endregion

        #endregion

        #region Methods

        public virtual void CheckForUserRole()
        {
            const string expectedRole = "DIT";
            WindowsPrincipal currentPrincipal = this.GetCurrentIdentity();
            if (!currentPrincipal.IsInRole(expectedRole))
            {
                throw new AuthenticationException(string.Format("User should be in '{0}' role.", expectedRole));
            }
        }

        private WindowsPrincipal GetCurrentIdentity()
        {
            return new WindowsPrincipal(OperationContext.Current.ServiceSecurityContext.WindowsIdentity);
        }

        public virtual void CheckForAdminRole()
        {
            const string expectedRole = "DIT";
            WindowsPrincipal currentPrincipal = this.GetCurrentIdentity();
            if (!currentPrincipal.IsInRole(expectedRole))
            {
                throw new AuthenticationException(string.Format("User should be in '{0}' role.", expectedRole));
            }
        }

        public virtual string GetLogin()
        {
            return Thread.CurrentPrincipal.Identity.Name;
        }

        #endregion
    }
}