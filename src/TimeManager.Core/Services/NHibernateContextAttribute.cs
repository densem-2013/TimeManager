namespace Infocom.TimeManager.Core.Services
{
    using System;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Description;
    using System.ServiceModel.Dispatcher;

    using NHibernate;

    public class NHibernateContextAttribute : Attribute, IContractBehavior
    {
        #region Properties

        public ISessionFactory SessionFactory { private get; set; }

        #endregion

        #region Implemented Interfaces

        #region IContractBehavior

        public void AddBindingParameters(
            ContractDescription contractDescription, 
            ServiceEndpoint endpoint, 
            BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyClientBehavior(
            ContractDescription contractDescription, ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
        }

        public void ApplyDispatchBehavior(
            ContractDescription contractDescription, ServiceEndpoint endpoint, DispatchRuntime dispatchRuntime)
        {
            dispatchRuntime.InstanceContextInitializers.Add(new NHibernateContextInitializer());
        }

        public void Validate(ContractDescription contractDescription, ServiceEndpoint endpoint)
        {
        }

        #endregion

        #endregion
    }
}