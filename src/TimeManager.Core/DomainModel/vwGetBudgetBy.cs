namespace Infocom.TimeManager.Core.DomainModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    using Infocom.TimeManager.Core.DomainModel;

    [PersistedEntity]
    public class vwGetBudgetBy : DomainObject
    {

        public vwGetBudgetBy()
        {
         
        }

        public virtual double SpentTime { get; set; }

        public virtual long ProjectId { get; set; }

        public virtual long DepartmentId { get; set; }

        public virtual double SpentMoney { get; set; }

    }
}