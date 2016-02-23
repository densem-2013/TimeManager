using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Infocom.TimeManager.WebAccess.ViewModels
{
    public class RequestDetailTypesViewModel : BaseViewModel
    {

        public RequestDetailTypesViewModel()
        {
            RequestDetailTypes = new List<RequestDetailModel>();
        }

        public List<RequestDetailModel> RequestDetailTypes { get; set; }
    }
}