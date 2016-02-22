namespace Infocom.TimeManager.WebAccess.ViewModels
{
    using System.Collections.Generic;

    using Infocom.TimeManager.Core.DomainModel;
    using Infocom.TimeManager.WebAccess.Models;

    public class ProjectDetailsEditViewModel
    {
        public IEnumerable<PhaseModel> Phases { get; set; }

        public IEnumerable<EmployeeModel> AllEmployees { get; set; }

        public IEnumerable<StatusModel> TaskStatuses { get; set; }

    }
}