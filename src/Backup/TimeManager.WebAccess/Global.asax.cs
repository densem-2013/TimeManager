namespace Infocom.TimeManager.WebAccess
{
    using System;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;

    using CommonServiceLocator.NinjectAdapter;

    using Infocom.TimeManager.Core;
    using Infocom.TimeManager.Core.DomainModel.Repositories;
    using Infocom.TimeManager.WebAccess.Controllers;

    using Microsoft.Practices.ServiceLocation;

    using NHibernate;
    using NHibernate.Context;

    using Ninject;
    using Ninject.Modules;
    using Ninject.Web.Common;

    public class MvcApplication : NinjectHttpApplication
    {
        #region Constructors and Destructors

        public MvcApplication()
        {
           // HibernatingRhinos.Profiler.Appender.NHibernate.NHibernateProfiler.Initialize();

            this.BeginRequest +=
                delegate { CurrentSessionContext.Bind(ServiceLocator.Current.GetInstance<ISessionFactory>().OpenSession()); };

            this.EndRequest += delegate
                {
                    System.Threading.Thread.Sleep(5);
                    var session = ServiceLocator.Current.GetInstance<ISessionFactory>().GetCurrentSession();
                    if (null != session)
                    {
                        session.Dispose();
                    }

                    CurrentSessionContext.Unbind(ServiceLocator.Current.GetInstance<ISessionFactory>());
                };
        }

        #endregion

        #region Public Methods

        public static void RegisterRoutes(RouteCollection routes)
        {
            var defaultControllerName = "TimeRegistration";
            
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default", 
                // Route name
                "{controller}/{action}/{id}", 
                // URL with parameters
                 //new { controller = "ProjectsOverview", action = "Index", id = UrlParameter.Optional }
                new { controller = defaultControllerName, action = "Index", id = UrlParameter.Optional }
                
                
                // Parameter defaults
                );
        }

        #endregion

        #region Methods

        protected void Application_Error(object sender, EventArgs e)
        {
            var exception = Server.GetLastError();
            // Log the exception.
            Response.Clear();

            var httpException = exception as HttpException;

            var routeData = new RouteData();
            routeData.Values.Add("controller", "Error");

            if (httpException == null)
            {
                routeData.Values.Add("action", "Index");
            }
            else //It's an Http Exception, Let's handle it.
            {
                switch (httpException.GetHttpCode())
                {
                    case 404:
                        // Page not found.
                        routeData.Values.Add("action", "HttpError404");
                        break;
                    case 505:
                        // Server error.
                        routeData.Values.Add("action", "HttpError505");
                        break;

                    // Here you can handle Views to other error codes.
                    // I choose a General error template  
                    default:
                        routeData.Values.Add("action", "Index");
                        break;
                }
            }

            // Pass exception details to the target error View.
            routeData.Values.Add("error", exception);

            // Clear the error on server.
            Server.ClearError();

            // Call target Controller and pass the routeData.
            IController errorController = new ErrorController();
            errorController.Execute(new RequestContext(
                 new HttpContextWrapper(Context), routeData));
        }

        protected override IKernel CreateKernel()
        {
            return new StandardKernel(new ServiceModule(), new PersistenceInjection());
        }

        protected override void OnApplicationStarted()
        {
            //HibernatingRhinos.Profiler.Appender.NHibernate.NHibernateProfiler.Initialize();
            AreaRegistration.RegisterAllAreas();
            RegisterRoutes(RouteTable.Routes);

            ServiceLocator.SetLocatorProvider(() => new NinjectServiceLocator(this.Kernel));
        }

        #endregion
    }

    internal class ServiceModule : NinjectModule
    {
        #region Public Methods

        public override void Load()
        {
            this.Bind<IProjectRepository>().To<ProjectRepository>();
            this.Bind<ITaskRepository>().To<TaskRepository>();
            this.Bind<IEmployeeRepository>().To<EmployeeRepository>();
            this.Bind<IStatusRepository>().To<StatusRepository>();
            this.Bind<ITimeRecordRepository>().To<TimeRecordRepository>();
           // this.Bind<IRequestRepository>().To<RequestRepository>();
        }

        #endregion
    }
}