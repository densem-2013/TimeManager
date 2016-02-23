namespace Infocom.TimeManager.WebAccess.ViewModels
{
    using System.Collections.Generic;

    using Infocom.TimeManager.Core.DomainModel;

    using Infocom.TimeManager.WebAccess.Models;

    public class TimeRecordViewModel : BaseViewModel
    {
        #region Properties

        public List<EmployeeModel> AssignedEmployees { get; set; }

        public TimeRecord TimeRecord { get; set; }

        #endregion
    }
}