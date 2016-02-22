namespace Infocom.TimeManager.WebAccess.Exceptions
{
    using System;
    using System.Runtime.Serialization;

    public class InfrastructureException : ApplicationException
    {
        #region Constructors and Destructors

        public InfrastructureException()
        {
        }

        public InfrastructureException(string message)
            : base(message)
        {
        }

        public InfrastructureException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected InfrastructureException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        #endregion
    }
}