using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;

namespace Infocom.TimeManager.WebAccess.Models
{
    public class TenderResultTypeModel : BaseModel
    {
        #region Properties

        [DisplayName(@"Наименование")]
        public string Name { get; set; }

        #endregion
    }
}