using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Infocom.TimeManager.WebAccess.ViewModels
{
    public class BudgetViewModel
    {
        public virtual double SpentMoney { get; set; }
        public virtual double SpentTime { get; set; }
    }
}