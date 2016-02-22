namespace Infocom.TimeManager.Core.Services
{
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Dispatcher;

    using Microsoft.Practices.ServiceLocation;

    using NHibernate;

    public class NHibernateContextInitializer : IInstanceContextInitializer
    {
        public void Initialize(InstanceContext instanceContext, Message message)
        {
            var newSession = ServiceLocator.Current.GetInstance<ISessionFactory>().OpenSession();
            instanceContext.Extensions.Add(new NHibernateContextExtension(newSession));
        }
    }
}
