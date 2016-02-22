namespace Infocom.TimeManager.WebAccess.ViewModels
{
    using System.Collections.Generic;


    using Infocom.TimeManager.WebAccess.Models;

    public class EmployeesViewModel
    {
        #region Properties
        public IEnumerable<EmployeeModel> Employees { get; set; }
        #endregion
    }
}