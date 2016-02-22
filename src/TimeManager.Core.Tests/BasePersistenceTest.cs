namespace Infocom.TimeManager.Core.Tests
{
    using Infocom.TimeManager.Core.Extensions;
    using Infocom.TimeManager.Core.Services;
    using Infocom.TimeManager.WebAccess.Tests;

    using Microsoft.Practices.ServiceLocation;

    using NHibernate;
    using NHibernate.Cfg;
    using NHibernate.Context;

    public abstract class BasePersistenceTest<T> : BaseTest<T> where T : class
    {
        #region Constructors and Destructors

        public BasePersistenceTest()
        {
            PersistenceInjection.SetServiceLocator();
        }

        #endregion

        #region Methods

        public override void TestFixtureSetUp()
        {
            base.TestFixtureSetUp();

            HibernatingRhinos.Profiler.Appender.NHibernate.NHibernateProfiler.Initialize();
        }

        public override void SetUp()
        {
            base.SetUp();

            CurrentSessionContext.Bind(ServiceLocator.Current.GetInstance<ISessionFactory>().OpenSession());
        }

        public override void TearDown()
        {
            base.TearDown();

            ServiceLocator.Current.GetInstance<ISessionFactory>().GetCurrentSession().Dispose();
        }

        protected virtual void InitializeDBSchema()
        {
            ServiceLocator.Current.GetInstance<Configuration>().InitializedDb();
        }

        #endregion
    }

    public abstract class BasePersistenceMockTest<T> : BaseMockTest<T> where T : class
    {
        #region Constructors and Destructors

        public BasePersistenceMockTest()
        {
            PersistenceInjection.SetServiceLocator();
        }

        #endregion

        #region Methods

        public override void TestFixtureSetUp()
        {
            base.TestFixtureSetUp();

            HibernatingRhinos.Profiler.Appender.NHibernate.NHibernateProfiler.Initialize();
        }

        public override void SetUp()
        {
            base.SetUp();

            CurrentSessionContext.Bind(ServiceLocator.Current.GetInstance<ISessionFactory>().OpenSession());
        }

        public override void TearDown()
        {
            base.TearDown();

            ServiceLocator.Current.GetInstance<ISessionFactory>().GetCurrentSession().Dispose();
        }

        protected virtual void InitializeDBSchema()
        {
            ServiceLocator.Current.GetInstance<Configuration>().InitializedDb();
        }

        #endregion
    }

}