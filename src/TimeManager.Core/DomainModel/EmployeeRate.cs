namespace Infocom.TimeManager.Core.DomainModel
{
    using System;
    using Infocom.TimeManager.Core.DomainModel.Validators;
    using NHibernate.Validator.Constraints;

    [PersistedEntity]
    public class EmployeeRate : DomainObject
    {
        #region Properties
        [NotNull]
        public virtual Employee Employee { get; set; }

        [NotNull]
        public virtual DateTime? StartDate { get; set; }

        public virtual DateTime? FinishDate { get; set; }

        [NotNull]
        public virtual decimal? Rate { get; set; }

        [NotNull]
        public virtual int RateNumber { get; set; }
        #endregion
    }
}