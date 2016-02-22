namespace Infocom.TimeManager.Core.Tests
{
    using Infocom.TimeManager.Core.Extensions;

    public class DBTools
    {
        protected void InitializeDBSchema()
        {
            new PersistenceInjection().GetNHibernateConfig().InitializedDb();
        }

    }


}