namespace Infocom.TimeManager.Core.Services
{
    using System;
    using System.ServiceModel;

    using Microsoft.Practices.ServiceLocation;

    using NHibernate;
    using NHibernate.Context;

    public class NHibernateContext
    {
        #region Constants and Fields

        private static object sync = new object();

        #endregion

        #region Public Methods

        public static NHibernateContextExtension Current()
        {
            lock (sync)
            {
                var context = OperationContext.Current;
                if (context == null)
                {
                    var sessionFactory = ServiceLocator.Current.GetInstance<ISessionFactory>();
                    try
                    {
                        return new NHibernateContextExtension(sessionFactory.GetCurrentSession());
                    }
                    catch (HibernateException)
                    {
                        CurrentSessionContext.Bind(ServiceLocator.Current.GetInstance<ISessionFactory>().OpenSession());
                        return new NHibernateContextExtension(sessionFactory.GetCurrentSession());
                    }
                }

                return context.InstanceContext.Extensions.Find<NHibernateContextExtension>();
            }
        }

        #endregion
    }
}