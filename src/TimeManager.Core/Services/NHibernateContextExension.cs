namespace Infocom.TimeManager.Core.Services
{
    using System;
    using System.ServiceModel;

    using NHibernate;

    public class NHibernateContextExtension : IExtension<InstanceContext>
    {
        public NHibernateContextExtension(ISession session)
        {
            Session = session;
        }

        public ISession Session { get; private set; }

        public void Attach(InstanceContext owner)
        {
        }

        public void Detach(InstanceContext owner)
        {
        }
    }
}
