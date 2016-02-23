namespace Infocom.TimeManager.Core.DomainModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    using Infocom.TimeManager.Core.DomainModel;

    [PersistedEntity]
    public class vwRequest: DomainObject
    {

        public vwRequest()
        {
         
        }

        public virtual string Number { get; set; }

        public virtual DateTime Date { get; set; }

        public virtual DateTime FinishDate { get; set; }

        public virtual Boolean IsValidated { get; set; }

        public virtual long ProjectManagerId { get; set; }

        public virtual string ProjectManagerShortName { get; set; }

        public virtual string ContractName { get; set; }

        public virtual string CustomerName { get; set; }

        public virtual string Detail { get; set; }

        public virtual string Documentation { get; set; }

        public virtual string ProcessState { get; set; }

        public virtual string TypeName { get; set; }

        public virtual long ProcessStateId { get; set; }

        public virtual long TypeId { get; set; }

        public virtual long ContractId { get; set; }

        public virtual long CustomerId { get; set; }

        public virtual long StatusId { get; set; }

        public virtual string StatusName { get; set; }

        public virtual DateTime LastUpdateDate { get; set; }

        public virtual long LastUpdateUserId { get; set; }

        public virtual string LastUpdateUserShortName { get; set; }

        public virtual DateTime StartDate { get; set; }

        public virtual string PhaseName { get; set; }
    }
}