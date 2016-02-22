using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;

namespace Infocom.TimeManager.WebAccess.ViewModels
{
    public class RequestDetailTypeModel : BaseViewModel
    {
        [DisplayName(@"Тип деталей заявки")]
        public long ID { get; set; }

        [DisplayName(@"Тип деталей заявки")]
        public string Name { get; set; }
    }
}