using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Infocom.TimeManager.WebAccess.Models;

namespace Infocom.TimeManager.WebAccess.ViewModels
{
    public class EmployeeRateViewModel
    {
        #region Properties

        public EmployeeModel Employee { get; set; }

        public IEnumerable<EmployeeRateModel> EmployeeRates { get; set; }

     #endregion
    }
}