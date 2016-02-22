namespace Infocom.TimeManager.WebAccess.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using System.Web.Security;
    using System.Globalization;
    using System.Threading;

    using Infocom.TimeManager.Core.DomainModel;
    using Infocom.TimeManager.Core.Services;
    using Infocom.TimeManager.WebAccess.Infrastructure;

    using NHibernate;
    using NHibernate.Linq;

    [CheckSession]
    [OptionalAuthorize]
    public abstract partial class TimeManagerBaseController : Controller
    {
        private IFutureValue<Employee> _currentEmployee;

        #region Properties

        public string UserName
        {
            get
            {
                return this.Session["userName"].ToString();
            }
        }

        internal virtual ISession PersistenceSession
        {
            get
            {
                return NHibernateContext.Current().Session;
            }
        }

        internal virtual IFutureValue<Employee> CurrentEmployee
        {
            get
            {
                return this._currentEmployee ??
                       (this._currentEmployee =
                        this.PersistenceSession.QueryOver<Employee>().Where(e => e.Login == this.UserName).FutureValue());
            }
        }

        #endregion

        #region Methods

        protected IEnumerable<Task> GetAssignedTasksForEmployee(Employee employee)
        {
            return this.PersistenceSession.Query<Task>().Where(t => t.AssignedEmployees.Contains(employee));
        }
        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);

            const string culture = "ru-RU";
            CultureInfo ci = CultureInfo.GetCultureInfo(culture);

            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;
        }

        #endregion
    }
}