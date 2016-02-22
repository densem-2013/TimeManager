namespace Infocom.TimeManager.WebAccess.ViewModels
{
    using System;
    using System.Collections.Generic;

    using Infocom.TimeManager.WebAccess.Models;

    public class EmployeeViewModel //: BaseViewModel
    {
        #region Constructors and Destructors

        public EmployeeViewModel()
        {
        }

        #endregion

        #region Properties

        public long ID { get; set; }

        public string ShortName { get; set; }
        
        #endregion
    }
}