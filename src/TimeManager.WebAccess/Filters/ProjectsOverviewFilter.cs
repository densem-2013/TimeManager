namespace Infocom.TimeManager.WebAccess.Filters
{
    public class ProjectsOverviewFilter
    {
        #region Properties

        public long FilteringByProjectTypeID { get; set; }

        public bool FilteringByAssignedProjects { get; set; }

        public string FilteringBySearchWord { get; set; }

        public long FilteringByStatusID { get; set; }

        #endregion
    }
}