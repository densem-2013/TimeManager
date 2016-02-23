using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Infocom.TimeManager.WebAccess.Models
{
    public class BudgetModel: BaseModel
    {
        #region Constructors and Destructors
        public BudgetModel()
        {
            this.BudgetAllocations = new List<BudgetAllocationModel>();
        }
        #endregion

        #region Properties

        public virtual List<BudgetAllocationModel> BudgetAllocations { get; set; }

        #endregion
    }
}