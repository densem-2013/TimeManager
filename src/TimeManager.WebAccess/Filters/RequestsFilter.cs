namespace Infocom.TimeManager.WebAccess.Filters
{
    using Infocom.TimeManager.WebAccess.Models;

    public class RequestsFilter
    {
        #region Properties

        public long FilteringByRequestTypeID { get; set; }

        public string FilteringBySearchWord { get; set; }

        public long FilteringByStatusID { get; set; }

        #endregion
    }
}