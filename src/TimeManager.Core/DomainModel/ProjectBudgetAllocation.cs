namespace Infocom.TimeManager.Core.DomainModel
{
    using System;
    [PersistedEntity]
    public class ProjectBudgetAllocation : DomainObject
    {
        #region Properties

        public virtual Department Department { get; set; }

        public virtual decimal? AmountOfMoney { get; set; }

        public virtual int? AmountOfHours { get; set; }

        public virtual DateTime? DateEndWorkByDepartment { get; set; }

        #endregion
    }
}