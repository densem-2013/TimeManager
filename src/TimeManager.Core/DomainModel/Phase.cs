namespace Infocom.TimeManager.Core.DomainModel
{
    using System;
    using System.Collections.Generic;

    using NHibernate.Validator.Constraints;

    [PersistedEntity]
    public class Phase : DomainObject
    {
        public Phase()
        {
            this.Budget = new ProjectBudget();
        }

        #region Properties

        [NotNull]
        public virtual int Number { get; set; }
        
        [NotNull]
        public virtual string Name { get; set; }
        
        [NotNull]
        public virtual DateTime? DeadLine { get; set; }
        
        public virtual ProjectBudget Budget { get; set; }

        public virtual Request Request { get; set; }

        public virtual ICollection<Project> Projects { get; set; }
        #endregion

    }
}
