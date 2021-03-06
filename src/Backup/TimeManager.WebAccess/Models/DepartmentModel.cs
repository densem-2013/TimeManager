﻿namespace Infocom.TimeManager.WebAccess.Models
{
   

    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    using Infocom.TimeManager.Core.DomainModel;
    using Infocom.TimeManager.WebAccess.ViewModels;
   
    public class DepartmentModel : BaseModel
    {
       
      
        #region Properties

        [DisplayName(@"Департамент")]
        public string ShortName { get; set; }

        public int RequestPermission { get; set; }

        public int TimeregPermission { get; set; }


    #endregion
    }
}