namespace Infocom.TimeManager.WebAccess.ViewModels
{
    using System.Collections;
    using System.Collections.Generic;

    using Infocom.TimeManager.Core.DomainModel;
    using Infocom.TimeManager.WebAccess.Models;

    public class RequestEditorViewModel
    {
        public IEnumerable<RequestTypeModel> RequestTypes { get; set; }

        public IEnumerable<EmployeeViewModel> Managers { get; set; }

        public IEnumerable<DepartmentModel> Departments { get; set; }

        public string RequestNumber { get; set; }

        public IEnumerable<RequestDetailTypeModel> RequestDetailTypes { get; set; }
    }
}