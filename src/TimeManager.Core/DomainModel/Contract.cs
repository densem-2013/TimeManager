namespace Infocom.TimeManager.Core.DomainModel
{
    using System;

    [PersistedEntity]
    public class Contract : DomainObject
    {
        public virtual string Name { get; set; }

        public virtual Customer Customer { get; set; }

        public virtual string Number { get; set; }

        public virtual string OldCustomer { get; set; }

        public virtual DateTime? SigningDate { get; set; }

        public virtual ContractType ContractType { get; set; }

        public virtual DateTime? CompletionDate { get; set; }
    }
}
