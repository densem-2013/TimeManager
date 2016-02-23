namespace Infocom.TimeManager.WebAccess.ViewModels
{
    using System.Collections.Generic;


    using Infocom.TimeManager.WebAccess.Models;

    public class ProjectsOverviewViewModel : BaseViewModel
    {
        #region Constructors and Destructors

        public ProjectsOverviewViewModel()
        {
            this.Projects = new List<ProjectModel>();
            this.AllEmployees = new List<EmployeeViewModel>();
        }

        #endregion

        #region Properties

        public IEnumerable<ProjectModel> Projects { get; set; }

        public IEnumerable<EmployeeViewModel> AllEmployees { get; set; }

        #endregion
    }
}