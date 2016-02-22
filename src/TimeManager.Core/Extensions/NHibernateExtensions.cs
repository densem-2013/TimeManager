namespace Infocom.TimeManager.Core.Extensions
{
    using System;

    using Infocom.IT.ILibrary.Logging.Extensions;

    using Microsoft.Practices.EnterpriseLibrary.Logging;

    using NHibernate;
    using NHibernate.Cfg;
    using NHibernate.Tool.hbm2ddl;

    public static class NHibernateExtensions
    {
        #region Public Methods

        public static void ExecuteAction(this ISession session, Action<ISession, ITransaction> action)
        {
            using (var tx = session.BeginTransaction())
            {
                action(session, tx);
                if (tx.IsActive)
                {
                    tx.Commit();
                }
            }
        }

        public static TResult ExecuteAction<TResult>(
            this ISession session, Func<ISession, ITransaction, TResult> action)
        {
            using (var tx = session.BeginTransaction())
            {
                TResult result = action(session, tx);

                tx.Commit();

                return result;
            }
        }

        public static void InitializedDb(this Configuration configuration)
        {
            var schemaExport = new SchemaExport(configuration);
            try
            {
                schemaExport.Drop(false, true);
            }
            catch (Exception ex)
            {
                //Logger.Writer.Write("Exception on droping db.", ex);
            }

            schemaExport.Create(false, true);
        }

        #endregion
    }
}