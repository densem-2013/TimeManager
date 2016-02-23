namespace Infocom.TimeManager.Core.DomainModel
{
    using System.Collections.Generic;

    [PersistedEntity]
    public class ProjectBudget : DomainObject
    {
        #region Constructors and Destructors

        public ProjectBudget()
        {
            this.BudgetAllocations = new List<ProjectBudgetAllocation>();
        }

        #endregion

        #region Properties

        public virtual ICollection<ProjectBudgetAllocation> BudgetAllocations { get; set; }

        #endregion
    }
}