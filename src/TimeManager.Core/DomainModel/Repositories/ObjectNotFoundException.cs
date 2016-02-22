namespace Infocom.TimeManager.Core.DomainModel.Repositories
{
    using System;
    using System.Runtime.Serialization;

    public class ObjectNotExistException : ApplicationException
    {
        #region Constructors and Destructors

        public ObjectNotExistException()
        {
        }

        public ObjectNotExistException(string message)
            : base(message)
        {
        }

        public ObjectNotExistException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected ObjectNotExistException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        #endregion
    }
}