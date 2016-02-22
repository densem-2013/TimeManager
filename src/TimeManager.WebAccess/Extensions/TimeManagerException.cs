namespace Infocom.TimeManager.WebAccess.Extensions
{
    using System;

    [Serializable]
    public class TimeManagerException:Exception
    {
        public TimeManagerException(string message)
            : base(message)
        {
        }
    }
}