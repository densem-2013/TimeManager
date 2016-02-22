namespace Infocom.TimeManager.WebAccess.ViewModels
{
    using System.Collections.Generic;

    using Infocom.TimeManager.WebAccess.Models;

    public class RequestDetailsViewModel : BaseViewModel
    {
         #region Constructors and Destructors

        public RequestDetailsViewModel()
        {
            this.Request = new RequestModel();
            this.Projects = new List<ProjectModel>();
        }

        #endregion

        #region Properties

        public RequestModel Request { get; set; }

        public IEnumerable<ProjectModel> Projects { get; set; }

        public IEnumerable<RequestDetailTypeModel> RequestDetailTypes { get; set; }

        //my
        public ProjectModel Project { get; set; }

        public IEnumerable<TaskModel> Tasks { get; set; }

        public EmployeeModel Empoloyee { get; set; }
        //my

        #endregion
    }
}