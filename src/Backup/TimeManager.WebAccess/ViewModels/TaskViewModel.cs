namespace Infocom.TimeManager.WebAccess.ViewModels
{
    using System;
    using System.Collections.Generic;

    using Infocom.TimeManager.WebAccess.Models;

    public class TaskViewModel : BaseViewModel
    {
        #region Constructors and Destructors

        public TaskViewModel(long employeeID)
        {
            this.Task = new TaskModel();
            this.AssignedEmployees = new List<EmployeeModel>();
            this.NotAssignedEmployees = new List<EmployeeModel>();
        }

        #endregion

        #region Properties

        public TaskModel Task { get; set; }

        public IEnumerable<EmployeeModel> AssignedEmployees { get; set; }

        public IEnumerable<EmployeeModel> NotAssignedEmployees { get; set; }

        public TimeSpan SpentTimeByMe { get; set; }

        public TimeSpan SpentTimeByAll { get; set; }

        public IEnumerable<TimeSheetOverviewModel> TimeSheetOverviews { get; set; }

        #endregion
    }
}