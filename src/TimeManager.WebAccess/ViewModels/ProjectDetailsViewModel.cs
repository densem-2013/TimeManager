namespace Infocom.TimeManager.WebAccess.ViewModels
{
    using System.Collections.Generic;


    using Infocom.TimeManager.WebAccess.Models;

    public class ProjectDetailsViewModel : BaseViewModel
    {
        #region Constructors and Destructors

        public ProjectDetailsViewModel()
        {
            this.Project = new ProjectModel();
            this.Tasks = new List<TaskModel>();
        }

        #endregion

        #region Properties

        public ProjectModel Project { get; set; }

        public IEnumerable<TaskModel> Tasks { get; set; }

        #endregion
    }
}