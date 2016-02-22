using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Infocom.TimeManager.WebAccess.Models
{
    using System.ComponentModel;

    public class RequestStatusModel : BaseModel
    {
        #region Properties

        [DisplayName(@"Статус")]
        public string Name { get; set; }

        #endregion
    }
}