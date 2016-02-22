namespace Infocom.TimeManager.Services.ConsoleHost
{
    using System;
    using System.Collections.Generic;
    using System.ServiceModel;

    using CommonServiceLocator.NinjectAdapter;

    using Infocom.TimeManager.Core;
    using Infocom.TimeManager.Core.Services;

    using Microsoft.Practices.ServiceLocation;

    using Ninject;

    internal class Program
    {
        #region Constants and Fields

        private static ICollection<ServiceHost> _services = new List<ServiceHost>();

        #endregion

        #region Methods

        private static void Main(string[] args)
        {
            HibernatingRhinos.Profiler.Appender.NHibernate.NHibernateProfiler.Initialize();

            InitializeServiceLocator();

            Host<TaskManagement>();

            WaitForKeyPress();
            Dispose();
        }

        private static void InitializeServiceLocator()
        {
            var container = new StandardKernel(new PersistenceInjection());
            ServiceLocator.SetLocatorProvider(() => new NinjectServiceLocator(container));
        }

        private static void WaitForKeyPress()
        {
            Console.WriteLine("Press <Enter> to close application.");
            while (Console.ReadKey(true).Key != ConsoleKey.Enter)
            {
            }
        }

        private static void Dispose()
        {
            foreach (var serviceHost in _services)
            {
                serviceHost.Abort();
            }
        }

        private static void Host<TService>()
        {
            var host = new ServiceHost(typeof(TService));
            _services.Add(host);
            Console.WriteLine("{0}: Starting", typeof(TService).Name);
            host.Open();
            Console.WriteLine("{0}: Started", typeof(TService).Name);

            foreach (var endpoint in host.Description.Endpoints)
            {
                Console.WriteLine("{0}: {1}", endpoint.Name, endpoint.Address);
            }
        }

        #endregion
    }
}