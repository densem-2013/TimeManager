namespace Infocom.TimeManager.WebAccess.ViewModels
{
    using System.Collections;
    using System.Collections.Generic;

    using Infocom.TimeManager.Core.DomainModel;
    using Infocom.TimeManager.WebAccess.Models;

    public class ProjectEditorViewModel
    {
        public IEnumerable<RequestShortModel> AllRequests { get; set; }

        public IEnumerable<RequestShortModel> NewRequests { get; set; }

        public IEnumerable<StatusModel> Statuses { get; set; }

        public IEnumerable<BudgetAllocationModel> BudgetAllocations { get; set; }

        public IEnumerable<EmployeeModel> AllEmployees { get; set; }

        public IEnumerable<EmployeeModel> ProjectManagers { get; set; }

        public IEnumerable<ProjectTypeModel> Types { get; set; }

        public IEnumerable<ShortRequestModel> Orders { get; set; }

        public IEnumerable<PhaseShortModel> Phases { get; set; }

        public IEnumerable<StatusModel> ViewStatuses { get; set; }

        public IEnumerable<ProjectPriorityModel> ProjectPriorities { get; set; }
    }
}